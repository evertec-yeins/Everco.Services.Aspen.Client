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
    using System.Net;
    using Assets;
    using Everco.Services.Aspen.Client.Auth;
    using Everco.Services.Aspen.Client.Tests.Identities;
    using Fluent;
    using Identity;
    using NUnit.Framework;
    using Providers;
    using IUserIdentity = Identity.IUserIdentity;

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
            IDelegatedApp client = this.GetDelegatedClient();
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
            IDelegatedApp client = this.GetDelegatedClient();
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
            IDelegatedApp client = this.GetDelegatedClient();
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
            IDelegatedApp client = this.GetDelegatedClient();
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
            IDelegatedApp client = this.GetDelegatedClient();
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().UpdateEnabled(appIdentity.ApiKey, false);
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("20006"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            StringAssert.IsMatch("ApiKey está desactivado. Póngase en contacto con el administrador", exception.Message);
            TestContext.CurrentContext.DatabaseHelper().UpdateEnabled(appIdentity.ApiKey, true);
        }

        /// <summary>
        /// Una solicitud a una operación que requiere firma no hay un token de autenticación vigente genera una respuesta inválida.
        /// </summary>
        [Test]
        [Category("Signed")]
        public void NotFoundValidTokenWhenUserSignedRequestThrows()
        {
            IDelegatedApp client = this.GetDelegatedClient();
            UserAuthToken authToken = client.AuthToken as UserAuthToken;
            Assert.That(authToken, Is.Not.Null);
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().RemoveUserAuthToken(appIdentity.ApiKey, userIdentity.DocType, userIdentity.DocNumber, authToken.DeviceId);
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
            IDelegatedApp client = this.GetDelegatedClient();
            UserAuthToken authToken = client.AuthToken as UserAuthToken;
            Assert.That(authToken, Is.Not.Null);
            Assert.That(authToken.DeviceId, Is.Not.Empty);
            Assert.That(authToken.Expired, Is.False);
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            IUserIdentity userIdentity = RecognizedUserIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().EnsureExpireUserAuthToken(appIdentity.ApiKey, userIdentity.DocType, userIdentity.DocNumber, authToken.DeviceId);

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
            IDelegatedApp client = this.GetDelegatedClient();
            IAppIdentity appIdentity = DelegatedAppIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().UpdateChangeSecret(appIdentity.ApiKey, true);
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("20009"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.UpgradeRequired));
            StringAssert.IsMatch("Necesita actualizar el secreto de la aplicación.", exception.Message);
            TestContext.CurrentContext.DatabaseHelper().UpdateChangeSecret(appIdentity.ApiKey, false);
        }

        /// <summary>
        /// Se produce una excepción de autenticación cuando se intenta usar un token de autenticación emitido para el usuario A vinculado a la aplicación A con el usuario B vinculado a la aplicación A.
        /// </summary>
        [Test]
        [Category("Signed")]
        public void MismatchTokenBetweenUsersWhenUserSignedRequestThrows()
        {
            IAppIdentity commonAppIdentity = DelegatedAppIdentity.Master;

            IUserIdentity userIdentityMaster = RecognizedUserIdentity.Master;
            IDelegatedApp clientAppMaster = DelegatedApp.Initialize(CachePolicy.BypassCache)
                .RoutingTo(TestingEndpointProvider.Default)
                .WithIdentity(commonAppIdentity)
                .Authenticate(userIdentityMaster)
                .GetClient();

            Assert.That(clientAppMaster, Is.Not.Null);
            Assert.That(clientAppMaster.AuthToken, Is.Not.Null);
            Assert.That(clientAppMaster.AuthToken.Token, Is.Not.Null);

            IUserIdentity userIdentityHelper = RecognizedUserIdentity.Helper;
            IDelegatedApp clientAppHelper = DelegatedApp.Initialize(CachePolicy.BypassCache)
                .RoutingTo(TestingEndpointProvider.Default)
                .WithIdentity(commonAppIdentity)
                .Authenticate(userIdentityHelper)
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
            IUserIdentity commonUserIdentity = RecognizedUserIdentity.Master;
            
            IAppIdentity appIdentityMaster = DelegatedAppIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().EnsureUserAndProfileInfo(appIdentityMaster.ApiKey, commonUserIdentity.DocType, commonUserIdentity.DocNumber);

            IDelegatedApp clientAppMaster = DelegatedApp.Initialize(CachePolicy.BypassCache)
                .RoutingTo(TestingEndpointProvider.Default)
                .WithIdentity(appIdentityMaster)
                .Authenticate(commonUserIdentity)
                .GetClient();

            Assert.That(clientAppMaster, Is.Not.Null);
            Assert.That(clientAppMaster.AuthToken, Is.Not.Null);
            Assert.That(clientAppMaster.AuthToken.Token, Is.Not.Null);

            IAppIdentity appIdentityHelper = DelegatedAppIdentity.Helper;
            TestContext.CurrentContext.DatabaseHelper().EnsureUserAndProfileInfo(appIdentityHelper.ApiKey, commonUserIdentity.DocType, commonUserIdentity.DocNumber);

            IDelegatedApp clientAppHelper = DelegatedApp.Initialize(CachePolicy.BypassCache)
                .RoutingTo(TestingEndpointProvider.Default)
                .WithIdentity(appIdentityHelper)
                .Authenticate(commonUserIdentity)
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
            IUserIdentity userIdentityMaster = RecognizedUserIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().EnsureUserAndProfileInfo(appIdentityMaster.ApiKey, userIdentityMaster.DocType, userIdentityMaster.DocNumber);
            IDelegatedApp clientAppMaster = DelegatedApp.Initialize(CachePolicy.BypassCache)
                .RoutingTo(TestingEndpointProvider.Default)
                .WithIdentity(appIdentityMaster)
                .Authenticate(userIdentityMaster)
                .GetClient();

            Assert.That(clientAppMaster, Is.Not.Null);
            Assert.That(clientAppMaster.AuthToken, Is.Not.Null);
            Assert.That(clientAppMaster.AuthToken.Token, Is.Not.Null);

            IAppIdentity appIdentityHelper = DelegatedAppIdentity.Helper;
            IUserIdentity userIdentityHelper = RecognizedUserIdentity.Helper;
            TestContext.CurrentContext.DatabaseHelper().EnsureUserAndProfileInfo(appIdentityHelper.ApiKey, userIdentityHelper.DocType, userIdentityHelper.DocNumber);
            IDelegatedApp clientAppHelper = DelegatedApp.Initialize(CachePolicy.BypassCache)
                .RoutingTo(TestingEndpointProvider.Default)
                .WithIdentity(appIdentityHelper)
                .Authenticate(userIdentityHelper)
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
            IDelegatedApp client = this.GetDelegatedClient();
            IUserIdentity userIdentityHelper = RecognizedUserIdentity.Helper;
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
            IDelegatedApp client = this.GetDelegatedClient();
            IAppIdentity appIdentityMaster = DelegatedAppIdentity.Master;
            IUserIdentity userIdentityMaster = RecognizedUserIdentity.Master;
            TestContext.CurrentContext.DatabaseHelper().EnsureMismatchUserAuthToken(appIdentityMaster.ApiKey, userIdentityMaster.DocType, userIdentityMaster.DocNumber, userIdentityMaster.Device.DeviceId);
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15846"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            StringAssert.IsMatch("No coinciden los datos recibidos del token vs los valores esperados. ¿Se modificaron los valores en tránsito o está utilizando el ApiKey en otra aplicación?", exception.Message);
        }
    }
}