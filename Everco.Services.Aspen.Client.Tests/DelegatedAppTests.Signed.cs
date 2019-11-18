// -----------------------------------------------------------------------
// <copyright file="DelegatedAppTests.Signed.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Assets;
    using Everco.Services.Aspen.Client.Auth;
    using Everco.Services.Aspen.Client.Tests.Identities;
    using Fluent;
    using NUnit.Framework;
    using Providers;

    /// <summary>
    /// Implementa las pruebas unitarias de la firma requerida para acceder a las operaciones que requieren autenticación.
    /// </summary>
    [TestFixture]
    public partial class DelegatedAppTests
    {
        /// <summary>
        /// Una solicitud a una operación que requiere firma cuando se usa un ApiKey no reconocido genera una respuesta inválida.
        /// </summary>
        [Test]
        [Category("Signed")]
        public void UnrecognizedApiKeyWhenAppSignedRequestThrows()
        {
            IDelegatedApp client = GetDelegatedClient();
            string unrecognizedApiKey = Guid.NewGuid().ToString();
            IHeadersManager apiKeyHeaderBehavior = InvalidApiKeyHeader.WithHeaderBehavior(() => unrecognizedApiKey);
            ServiceLocator.Instance.RegisterHeadersManager(apiKeyHeaderBehavior);
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("20005"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Identificador de ApiKey no válido para la cabecera personalizada 'X-PRO-Auth-App'", exception.Message);
        }

        /// <summary>
        /// Una solicitud a una operación que requiere firma si el secreto de la aplicación no es válido se genera una respuesta inválida.
        /// </summary>
        [Test]
        [Category("Signed")]
        public void InvalidAppCredentialWhenAppSignedRequestThrows()
        {
            IDelegatedApp client = GetDelegatedClient();
            string invalidApiSecret = Guid.NewGuid().ToString();
            IHeadersManager invalidAppSecretBehavior = InvalidPayloadHeader.WithCustomAppSecret(invalidApiSecret);
            ServiceLocator.Instance.RegisterHeadersManager(invalidAppSecretBehavior);
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("20007"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("El contenido de la cabecera personalizada 'X-PRO-Auth-Payload' no es válido. Invalid signature. ¿Está utilizando las credenciales proporcionadas?", exception.Message);
        }

        /// <summary>
        /// Se produce una excepción cuando el identificador del dispositivo no coincide con el usado en la autenticación.
        /// </summary>
        [Test]
        [Category("Signed")]
        public void MismatchDeviceIdWhenUserSignedRequestThrows()
        {
            IDelegatedApp client = GetDelegatedClient();
            IPayloadClaimsManager randomDeviceIdClaimBehavior = InvalidDeviceIdPayloadClaim.WithClaimBehavior(() => $"MyRandomDevice-{new Random().Next(999999, 9999999)}");
            ServiceLocator.Instance.RegisterPayloadClaimsManager(randomDeviceIdClaimBehavior);
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15847"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            StringAssert.IsMatch("No hay un token de autenticación vigente.", exception.Message);
        }

        /// <summary>
        /// Se produce una excepción cuando se usa un nombre de usuario para la firma que no existe en el sistema.
        /// </summary>
        [Test]
        [Category("Signed")]
        public void NotFoundUsernameWhenUserSignedRequestThrows()
        {
            IDelegatedApp client = GetDelegatedClient();
            string fixedDocType = "CC";
            string randomDocNumber = new Random().Next(1000000000, int.MaxValue).ToString();
            IPayloadClaimsManager randomUsernameClaimBehavior = InvalidUsernamePayloadClaim.WithClaimBehavior(() => $"{fixedDocType}-{randomDocNumber}");
            ServiceLocator.Instance.RegisterPayloadClaimsManager(randomUsernameClaimBehavior);
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15847"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            StringAssert.IsMatch("No hay un token de autenticación vigente.", exception.Message);
        }

        /// <summary>
        /// Una solicitud a una operación que requiere firma cuando la aplicación fue deshabilitada genera una respuesta inválida.
        /// </summary>
        [Test]
        [Category("Signed")]
        public void ApiKeyDisabledWhenUserSignedRequestThrows()
        {
            IDelegatedApp client = GetDelegatedClient();
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            SqlDataContext.DisableApp(appIdentity.ApiKey);
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("20006"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            StringAssert.IsMatch("ApiKey está desactivado. Póngase en contacto con el administrador", exception.Message);
            SqlDataContext.EnableApp(appIdentity.ApiKey);
        }

        /// <summary>
        /// Una solicitud a una operación que requiere firma no hay un token de autenticación vigente genera una respuesta inválida.
        /// </summary>
        [Test]
        [Category("Signed")]
        public void NotFoundValidTokenWhenUserSignedRequestThrows()
        {
            IDelegatedApp client = GetDelegatedClient();
            UserAuthToken authToken = client.AuthToken as UserAuthToken;
            Assert.That(authToken, Is.Not.Null);
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            IUserIdentity userIdentity = UserIdentity.Master;
            SqlDataContext.RemoveUserAuthToken(
                userIdentity.DocType,
                userIdentity.DocNumber,
                authToken.DeviceId,
                appIdentity.ApiKey);
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15847"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            StringAssert.IsMatch("No hay un token de autenticación vigente", exception.Message);
        }

        /// <summary>
        /// Se produce una excepción cuando el token de autenticación proporcionado en la firma ya caducó.
        /// </summary>
        [Test]
        [Category("Signed")]
        public void TokenProvidedExpiredWhenUserSignedRequestThrows()
        {
            IDelegatedApp client = GetDelegatedClient();
            UserAuthToken authToken = client.AuthToken as UserAuthToken;
            Assert.That(authToken, Is.Not.Null);
            Assert.That(authToken.DeviceId, Is.Not.Empty);
            Assert.That(authToken.Expired, Is.False);
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            IUserIdentity userIdentity = UserIdentity.Master;
            SqlDataContext.EnsureExpireUserAuthToken(
                userIdentity.DocType,
                userIdentity.DocNumber,
                authToken.DeviceId,
                appIdentity.ApiKey);

            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15848"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            StringAssert.IsMatch("El token de autenticación proporcionado ya venció", exception.Message);
        }

        /// <summary>
        /// Se produce una excepción de autenticación cuando la aplicación requiere cambiar su secreto.
        /// </summary>
        [Test]
        [Category("Signed")]
        public void AppRequiresChangeSecretWhenUserSignedRequestThrows()
        {
            IDelegatedApp client = GetDelegatedClient();
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            SqlDataContext.EnsureAppRequiresChangeSecret(appIdentity.ApiKey);
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("20009"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.UpgradeRequired));
            StringAssert.IsMatch("Necesita actualizar el secreto de la aplicación.", exception.Message);
            SqlDataContext.EnsureAppNotRequiresChangeSecret(appIdentity.ApiKey);
        }

        /// <summary>
        /// Se produce una excepción de autenticación cuando se intenta usar un token de autenticación emitido para el usuario A vinculado a la aplicación A con el usuario B vinculado a la aplicación A.
        /// </summary>
        [Test]
        [Category("Signed")]
        public void MismatchTokenBetweenUsersWhenUserSignedRequestThrows()
        {
            IAppIdentity commonAppIdentity = DelegatedAppIdentity.Master;

            IUserIdentity userIdentityMaster = UserIdentity.Master;
            IDelegatedApp clientAppMaster = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(commonAppIdentity)
                .AuthenticateNoCache(userIdentityMaster)
                .GetClient();

            Assert.That(clientAppMaster, Is.Not.Null);
            Assert.That(clientAppMaster.AuthToken, Is.Not.Null);
            Assert.That(clientAppMaster.AuthToken.Token, Is.Not.Null);

            IUserIdentity userIdentityHelper = UserIdentity.Helper;
            IDelegatedApp clientAppHelper = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(commonAppIdentity)
                .AuthenticateNoCache(userIdentityHelper)
                .GetClient();

            Assert.That(clientAppHelper, Is.Not.Null);
            Assert.That(clientAppHelper.AuthToken, Is.Not.Null);
            Assert.That(clientAppHelper.AuthToken.Token, Is.Not.Null);

            IPayloadClaimsManager mismatchTokenClaimBehavior = InvalidTokenPayloadClaim.WithClaimBehavior(() => clientAppHelper.AuthToken.Token);
            ServiceLocator.Instance.RegisterPayloadClaimsManager(mismatchTokenClaimBehavior);
            AspenException exception = Assert.Throws<AspenException>(() => clientAppMaster.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15846"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            StringAssert.IsMatch("No coinciden los datos recibidos del token vs los valores esperados. ¿Se modificaron los valores en tránsito o está utilizando el ApiKey en otra aplicación?", exception.Message);
        }

        /// <summary>
        /// Se produce una excepción de autenticación cuando se intenta usar un token de autenticación emitido para el usuario A vinculado a la aplicación A con el usuario A vinculado a la aplicación B.
        /// </summary>
        [Test]
        [Category("Signed")]
        public void MismatchBetweenAppsWhenUserSignedRequestThrows()
        {
            IUserIdentity commonUserIdentity = UserIdentity.Master;
            
            IAppIdentity appIdentityMaster = DelegatedAppIdentity.Master;
            SqlDataContext.EnsureUserAndProfileInfo(
                commonUserIdentity.DocType,
                commonUserIdentity.DocNumber,
                appIdentityMaster.ApiKey);

            IDelegatedApp clientAppMaster = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(appIdentityMaster)
                .AuthenticateNoCache(commonUserIdentity)
                .GetClient();

            Assert.That(clientAppMaster, Is.Not.Null);
            Assert.That(clientAppMaster.AuthToken, Is.Not.Null);
            Assert.That(clientAppMaster.AuthToken.Token, Is.Not.Null);

            IAppIdentity appIdentityHelper = DelegatedAppIdentity.Helper;
            SqlDataContext.EnsureUserAndProfileInfo(
                commonUserIdentity.DocType,
                commonUserIdentity.DocNumber,
                appIdentityHelper.ApiKey);

            IDelegatedApp clientAppHelper = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(appIdentityHelper)
                .AuthenticateNoCache(commonUserIdentity)
                .GetClient();

            Assert.That(clientAppHelper, Is.Not.Null);
            Assert.That(clientAppHelper.AuthToken, Is.Not.Null);
            Assert.That(clientAppHelper.AuthToken.Token, Is.Not.Null);

            IPayloadClaimsManager mismatchTokenClaimBehavior = InvalidTokenPayloadClaim.WithClaimBehavior(() => clientAppHelper.AuthToken.Token);
            ServiceLocator.Instance.RegisterPayloadClaimsManager(mismatchTokenClaimBehavior);
            AspenException exception = Assert.Throws<AspenException>(() => clientAppMaster.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15846"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            StringAssert.IsMatch("No coinciden los datos recibidos del token vs los valores esperados. ¿Se modificaron los valores en tránsito o está utilizando el ApiKey en otra aplicación?", exception.Message);
        }

        /// <summary>
        /// Se produce una excepción de autenticación cuando se intenta usar un token de autenticación emitido para el usuario A vinculado a la aplicación A con el usuario B vinculado a la aplicación B.
        /// </summary>
        [Test]
        [Category("Signed")]
        public void MismatchTokenBetweenAppsAndUsersWhenUserSignedRequestThrows()
        {
            IAppIdentity appIdentityMaster = DelegatedAppIdentity.Master;
            IUserIdentity userIdentityMaster = UserIdentity.Master;
            SqlDataContext.EnsureUserAndProfileInfo(
                userIdentityMaster.DocType,
                userIdentityMaster.DocNumber,
                appIdentityMaster.ApiKey);
            IDelegatedApp clientAppMaster = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(appIdentityMaster)
                .AuthenticateNoCache(userIdentityMaster)
                .GetClient();

            Assert.That(clientAppMaster, Is.Not.Null);
            Assert.That(clientAppMaster.AuthToken, Is.Not.Null);
            Assert.That(clientAppMaster.AuthToken.Token, Is.Not.Null);

            IAppIdentity appIdentityHelper = DelegatedAppIdentity.Helper;
            IUserIdentity userIdentityHelper = UserIdentity.Helper;
            SqlDataContext.EnsureUserAndProfileInfo(
                userIdentityHelper.DocType,
                userIdentityHelper.DocNumber,
                appIdentityHelper.ApiKey);
            IDelegatedApp clientAppHelper = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(appIdentityHelper)
                .AuthenticateNoCache(userIdentityHelper)
                .GetClient();

            Assert.That(clientAppHelper, Is.Not.Null);
            Assert.That(clientAppHelper.AuthToken, Is.Not.Null);
            Assert.That(clientAppHelper.AuthToken.Token, Is.Not.Null);

            IPayloadClaimsManager mismatchTokenClaimBehavior = InvalidTokenPayloadClaim.WithClaimBehavior(() => clientAppHelper.AuthToken.Token);
            ServiceLocator.Instance.RegisterPayloadClaimsManager(mismatchTokenClaimBehavior);
            AspenException exception = Assert.Throws<AspenException>(() => clientAppMaster.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15846"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            StringAssert.IsMatch("No coinciden los datos recibidos del token vs los valores esperados. ¿Se modificaron los valores en tránsito o está utilizando el ApiKey en otra aplicación?", exception.Message);
        }

        /// <summary>
        /// Se produce una excepción de autenticación cuando se intenta usar un token de autenticación emitido para el usuario A con el username del usuario B.
        /// </summary>
        [Test]
        [Category("Signed")]
        public void MismatchUsernameWhenUserSignedRequestThrows()
        {
            IDelegatedApp client = GetDelegatedClient();
            IUserIdentity userIdentityHelper = UserIdentity.Helper;
            IPayloadClaimsManager mismatchUsernameClaimBehavior = InvalidUsernamePayloadClaim.WithClaimBehavior(() => $"{userIdentityHelper.DocType}-{userIdentityHelper.DocNumber}");
            ServiceLocator.Instance.RegisterPayloadClaimsManager(mismatchUsernameClaimBehavior);
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15846"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            StringAssert.IsMatch("No coinciden los datos recibidos del token vs los valores esperados. ¿Se modificaron los valores en tránsito o está utilizando el ApiKey en otra aplicación?", exception.Message);
        }

        /// <summary>
        /// Se produce una excepción de autenticación cuando no coincide el token emitido por un usuario vs el token enviado en la firma.
        /// </summary>
        [Test]
        [Category("Signed")]
        public void MismatchTokenWhenUserSignedRequestThrows()
        {
            IDelegatedApp client = GetDelegatedClient();
            IAppIdentity appIdentityMaster = DelegatedAppIdentity.Master;
            IUserIdentity userIdentityMaster = UserIdentity.Master;
            SqlDataContext.EnsureMismatchUserAuthToken(
                userIdentityMaster.DocType,
                userIdentityMaster.DocNumber,
                userIdentityMaster.DeviceInfo.DeviceId,
                appIdentityMaster.ApiKey);
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15846"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            StringAssert.IsMatch("No coinciden los datos recibidos del token vs los valores esperados. ¿Se modificaron los valores en tránsito o está utilizando el ApiKey en otra aplicación?", exception.Message);
        }
    }
}