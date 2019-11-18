// -----------------------------------------------------------------------
// <copyright file="SqlDataContext.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-11-07 16:45 PM</date>
// -----------------------------------------------------------------------
// ReSharper disable ConvertToNullCoalescingCompoundAssignment
// ReSharper disable ConvertToUsingDeclaration
namespace Everco.Services.Aspen.Client.Tests.Assets
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Dynamic;
    using System.Linq;

    using Microsoft.Extensions.Configuration;

    using Newtonsoft.Json;

    /// <summary>
    /// Expone métodos para interactuar con la base de datos del servicio Aspen para fines de preparación de los escenarios de las pruebas automáticas.
    /// </summary>
    [SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "Keep calm and ignore")]
    public static class SqlDataContext
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="SqlDataContext"/> class from being created.
        /// </summary>
        static SqlDataContext()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{Environment.UserName}.json", true, true);
            IConfigurationRoot configuration = builder.Build();
            ConnectionString = configuration.GetConnectionString("Sql:Aspen");
        }

        /// <summary>
        /// Obtiene la cadena de conexión usada para el acceso al origen de datos.
        /// </summary>
        public static string ConnectionString { get; }

        /// <summary>
        /// Crea un usuario en el sistema; si el usuario ya existe, se asegura que no esté bloqueado.
        /// </summary>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        public static void EnsureUserInfo(string docType, string docNumber)
        {
            int userId = GetUserId(docType, docNumber);
            if (userId > 0)
            {
                EnsureUserIsNotLocked(docType, docNumber);
                return;
            }

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string commandText = @"
INSERT INTO [dbo].[Users] (
	[DocType],
	[DocNumber],
	[IsLockedOut],
	[LastLoginDate],
	[LastPasswordChangedDate],
	[LastLockoutDate],
	[FailedPasswordAttemptCount],
	[FailedPasswordAttemptWindowStart])
VALUES (@DocType, @DocNumber, 0, NULL, NULL, NULL, 0, NULL)
";
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("DocType", docType);
                    command.Parameters.AddWithValue("DocNumber", docNumber);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Crea un usuario con un perfil predeterminado en el sistema; si el usuario ya existe, se asegura que no esté bloqueado y se sobreescriben los datos del perfil.
        /// </summary>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        /// <param name="appKey">El identificador de la aplicación.</param>
        public static void EnsureUserAndProfileInfo(
            string docType,
            string docNumber,
            string appKey)
        {
            EnsureUserInfo(docType, docNumber);
            int userId = GetUserId(docType, docNumber);
            int appId = GetAppId(appKey);
            var pinNumberInfo = new
                                    {
                                        PinNumber = "141414",
                                        PinFormat = "Clear",
                                        DeviceId = $@"{Environment.UserDomainName}\{Environment.UserName}",
                                        LastUpdateAt = DateTime.Now
                                    };
            Dictionary<string, string> profileProperties = new Dictionary<string, string>()
                                                               {
                                                                   { "LastName", "Maribel" },
                                                                   { "FirstName", "Marquez Sosa" },
                                                                   { "Secret", "colombia" },
                                                                   { "SecretFormat", "Clear" },
                                                                   { "Email", "daniel.montalvo@evertecinc.com" },
                                                                   { "PinMetadata", JsonConvert.SerializeObject(pinNumberInfo, Formatting.None) }
                                                               };
            AddUserProfileProperties(userId, appId, profileProperties);
        }

        /// <summary>
        /// Crea un usuario con un perfil predeterminado en el sistema; si el usuario ya existe, se asegura que no esté bloqueado y se sobreescriben los datos del perfil.
        /// </summary>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        /// <param name="appKey">El identificador de la aplicación.</param>
        /// <param name="profileProperties">Una colección de claves y valores que representa las propiedades del perfil del usuario.</param>
        public static void EnsureUserAndProfileInfo(
            string docType,
            string docNumber,
            string appKey,
            IDictionary<string, string> profileProperties)
        {
            EnsureUserInfo(docType, docNumber);
            int userId = GetUserId(docType, docNumber);
            int appId = GetAppId(appKey);
            AddUserProfileProperties(userId, appId, profileProperties);
        }

        /// <summary>
        /// Inhabilita una aplicación en el sistema del servicio.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        public static void DisableApp(string appKey)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string commandText = @"
UPDATE [dbo].[Apps]
   SET [Enabled] = 0
WHERE [AppKey] = @AppKey
";
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("AppKey", appKey);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Habilita una aplicación en el sistema del servicio.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        public static void EnableApp(string appKey)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string commandText = @"
UPDATE [dbo].[Apps]
   SET [Enabled] = 1
WHERE [AppKey] = @AppKey
";
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("AppKey", appKey);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Se asegura una aplicación no requiera cambio de su secreto.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        public static void EnsureAppNotRequiresChangeSecret(string appKey)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string commandText = @"
UPDATE [dbo].[Apps]
   SET [UpdateSecret] = 0
WHERE [AppKey] = @AppKey
";
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("AppKey", appKey);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Se asegura una aplicación requiera cambio de su secreto.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        public static void EnsureAppRequiresChangeSecret(string appKey)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string commandText = @"
UPDATE [dbo].[Apps]
   SET [UpdateSecret] = 1
WHERE [AppKey] = @AppKey
";
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("AppKey", appKey);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Se asegura que el token de autenticación generado por una aplicación haya expirado.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        public static void EnsureExpireAppAuthToken(string appKey)
        {
            int appId = GetAppId(appKey);
            int randomDays = new Random().Next(2, 10);
            DateTimeOffset expiresAt = DateTimeOffset.UtcNow.AddDays(-randomDays);
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string commandText = @"
UPDATE [dbo].[AuthTokenApp]
   SET [ExpiresAt] = @ExpiresAt
WHERE [AppId] = @AppId
";
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("AppId", appId);
                    command.Parameters.AddWithValue("ExpiresAt", expiresAt);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Se asegura que el token de autenticación de un usuario haya expirado.
        /// </summary>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        /// <param name="deviceId">El identificador del dispositivo.</param>
        /// <param name="appKey">El identificador de la aplicación.</param>
        public static void EnsureExpireUserAuthToken(
            string docType,
            string docNumber,
            string deviceId,
            string appKey)
        {
            int userId = GetUserId(docType, docNumber);
            int appId = GetAppId(appKey);
            int randomDays = new Random().Next(2, 10);
            DateTimeOffset expiresAt = DateTimeOffset.UtcNow.AddDays(-randomDays);
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string commandText = @"
UPDATE [dbo].[AuthTokenUser]
   SET [ExpiresAt] = @ExpiresAt
 WHERE [AppId] = @AppId AND [UserId] = @UserId AND [DeviceId] = @DeviceId;
";
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("AppId", appId);
                    command.Parameters.AddWithValue("UserId", userId);
                    command.Parameters.AddWithValue("DeviceId", deviceId);
                    command.Parameters.AddWithValue("ExpiresAt", expiresAt);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Se asegura de alterar el token de autenticación de una aplicación.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        public static void EnsureMismatchAppAuthToken(string appKey)
        {
            int appId = GetAppId(appKey);
            string randomToken = $"{Guid.NewGuid():N}{Guid.NewGuid():N}";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string commandText = @"
UPDATE [dbo].[AuthTokenApp]
   SET [Token] = @Token
 WHERE [AppId] = @AppId;
";
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("AppId", appId);
                    command.Parameters.AddWithValue("Token", randomToken);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Se asegura de alterar el token de autenticación de un usuario.
        /// </summary>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        /// <param name="deviceId">El identificador del dispositivo.</param>
        /// <param name="appKey">El identificador de la aplicación.</param>
        public static void EnsureMismatchUserAuthToken(
            string docType,
            string docNumber,
            string deviceId,
            string appKey)
        {
            int userId = GetUserId(docType, docNumber);
            int appId = GetAppId(appKey);
            string randomToken = $"{Guid.NewGuid():N}{Guid.NewGuid():N}";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string commandText = @"
UPDATE [dbo].[AuthTokenUser]
   SET [Token] = @Token
 WHERE [AppId] = @AppId AND [UserId] = @UserId AND [DeviceId] = @DeviceId;
";
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("AppId", appId);
                    command.Parameters.AddWithValue("UserId", userId);
                    command.Parameters.AddWithValue("DeviceId", deviceId);
                    command.Parameters.AddWithValue("Token", randomToken);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Se asegura que el usuario este bloqueado.
        /// </summary>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        public static void EnsureUserIsLocked(string docType, string docNumber)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string commandText = @"
UPDATE [dbo].[Users]
	SET
		[LastLockoutDate] = @LastLockoutDate,
		[FailedPasswordAttemptCount] = @FailedPasswordAttemptCount,
		[FailedPasswordAttemptWindowStart] = @FailedPasswordAttemptWindowStart,
		[IsLockedOut] = @IsLockedOut
	WHERE [DocType] = @DocType AND [DocNumber] = @DocNumber
";
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("DocType", docType);
                    command.Parameters.AddWithValue("DocNumber", docNumber);
                    command.Parameters.AddWithValue("LastLockoutDate", DateTimeOffset.UtcNow);
                    command.Parameters.AddWithValue("FailedPasswordAttemptWindowStart", DateTimeOffset.UtcNow);
                    command.Parameters.AddWithValue("FailedPasswordAttemptCount", 10);
                    command.Parameters.AddWithValue("IsLockedOut", true);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Se asegura que el usuario no este bloqueado.
        /// </summary>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        public static void EnsureUserIsNotLocked(string docType, string docNumber)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string commandText = @"
UPDATE [dbo].[Users]
	SET
		[LastLockoutDate] = @LastLockoutDate,
		[FailedPasswordAttemptCount] = @FailedPasswordAttemptCount,
		[FailedPasswordAttemptWindowStart] = @FailedPasswordAttemptWindowStart,
        [LastPasswordChangedDate] = @LastPasswordChangedDate,
		[IsLockedOut] = @IsLockedOut
	WHERE [DocType] = @DocType AND [DocNumber] = @DocNumber
";
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("DocType", docType);
                    command.Parameters.AddWithValue("DocNumber", docNumber);
                    command.Parameters.AddWithValue("LastLockoutDate", DBNull.Value);
                    command.Parameters.AddWithValue("FailedPasswordAttemptCount", 0);
                    command.Parameters.AddWithValue("FailedPasswordAttemptWindowStart", DBNull.Value);
                    command.Parameters.AddWithValue("LastPasswordChangedDate", DBNull.Value);
                    command.Parameters.AddWithValue("IsLockedOut", false);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Obtiene el identificador de la aplicación en el sistema.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        /// <returns>El identificador de la aplicación en el sistema.</returns>
        public static int GetAppId(string appKey)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string commandText = @"
SELECT
	[AppId]
FROM [dbo].[Apps]
WHERE [AppKey] = @AppKey
";
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("AppKey", appKey);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        reader.Read();
                        return reader.HasRows ? reader.GetInt32("AppId") : 0;
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene el número máximo de intentos de autenticación inválidos permitidos, antes de bloquear a un usuario.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        /// <returns>Núméro máximo de intentos de autenticación inválidos permitidos</returns>
        public static int GetAppMaxFailedPasswordAttempt(string appKey)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string commandText = @"
SELECT
	[MaxFailedPasswordAttempt]
FROM [dbo].[Apps]
WHERE [AppKey] = @AppKey
";
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("AppKey", appKey);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        reader.Read();
                        return reader.HasRows ? reader.GetInt32("MaxFailedPasswordAttempt") : 0;
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene el identificador de un usuario en el sistema.
        /// </summary>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        /// <returns>El identificador del usuario en el sistema.</returns>
        public static int GetUserId(string docType, string docNumber)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string commandText = @"
SELECT [UserId]
FROM [dbo].[Users]
WHERE [DocType] = @DocType AND [DocNumber] = @DocNumber
";
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("DocType", docType);
                    command.Parameters.AddWithValue("DocNumber", docNumber);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        reader.Read();
                        return reader.HasRows ? reader.GetInt32("UserId") : 0;
                    }
                }
            }
        }

        /// <summary>
        /// Elimina el token de autenticación emitido por una aplicación.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        public static void RemoveAppAuthToken(string appKey)
        {
            int appId = GetAppId(appKey);
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string commandText = @"
DELETE FROM [dbo].[AuthTokenApp]
      WHERE [AppId] = @AppId
";
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("AppId", appId);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Elimina el token de autenticación emitido por un usuario.
        /// </summary>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        /// <param name="deviceId">El identificador del dispositivo.</param>
        /// <param name="appKey">El identificador de la aplicación.</param>
        public static void RemoveUserAuthToken(
            string docType,
            string docNumber,
            string deviceId,
            string appKey)
        {
            int appId = GetAppId(appKey);
            int userId = GetUserId(docType, docNumber);
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string commandText = @"
DELETE FROM [dbo].[AuthTokenUser]
    WHERE [AppId] = @AppId AND [UserId] = @UserId AND [DeviceId] = @DeviceId;
";
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("AppId", appId);
                    command.Parameters.AddWithValue("UserId", userId);
                    command.Parameters.AddWithValue("DeviceId", deviceId);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Remueve a un usuario del sistema.
        /// </summary>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        public static void RemoveUserInfo(string docType, string docNumber)
        {
            int userId = GetUserId(docType, docNumber);
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string commandText = @"
DELETE FROM [dbo].[UserProfile]
	WHERE [UserId] = @UserId;
DELETE FROM [dbo].[Users]
    WHERE [UserId] = @UserId;
";
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("UserId", userId);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Agrega las propiedades del perfil a un usuario.
        /// </summary>
        /// <param name="userId">El identificador del usuario en el sistema.</param>
        /// <param name="appId">El identificador de la aplicación en el sistema.</param>
        /// <param name="profileProperties">Una colección de claves y valores que representa las propiedades del perfil del usuario.</param>
        private static void AddUserProfileProperties(
            int userId,
            int appId,
            IDictionary<string, string> profileProperties)
        {
            if (profileProperties == null || !profileProperties.Any())
            {
                throw new ArgumentNullException($"Se requieren las propiedades de perfil para el usuario.");
            }

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                foreach ((string propertyName, string propertyValue) in profileProperties)
                {
                    using (SqlCommand command = new SqlCommand("[dbo].[SetUserProfileProperty]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("AppId", appId);
                        command.Parameters.AddWithValue("UserId", userId);
                        command.Parameters.AddWithValue("PropertyName", propertyName);
                        command.Parameters.AddWithValue("PropertyValue", propertyValue);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}