// -----------------------------------------------------------------------
// <copyright file="SqlServerHelper.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-11-07 16:45 PM</date>
// -----------------------------------------------------------------------
// ReSharper disable ConvertToNullCoalescingCompoundAssignment
// ReSharper disable ConvertToUsingDeclaration
namespace Everco.Services.Aspen.Client.Tests.Data
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Expone métodos para interactuar con la base de datos del servicio Aspen para fines de preparación de los escenarios de las pruebas automáticas.
    /// </summary>
    public class SqlServerHelper : IDatabaseHelper
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="SqlServerHelper"/> class from being created.
        /// </summary>
        public SqlServerHelper()
        {
            this.ConnectionString = Environment.GetEnvironmentVariable("ASPEN:ConnectionString", EnvironmentVariableTarget.Machine);

            if (!string.IsNullOrWhiteSpace(this.ConnectionString))
            {
                return;
            }

            const string ErrorMessage = "Missing or invalid environment variable ASPEN:ConnectionString";
            throw new ConfigurationErrorsException(ErrorMessage);
        }

        /// <summary>
        /// Obtiene la cadena de conexión usada para el acceso al origen de datos.
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// Agrega las propiedades del perfil a un usuario.
        /// </summary>
        /// <param name="userId">El identificador del usuario en el sistema.</param>
        /// <param name="appId">El identificador de la aplicación en el sistema.</param>
        /// <param name="profileProperties">Una colección de claves y valores que representa las propiedades del perfil del usuario.</param>
        public void AddUserProfileProperties(
            int userId,
            int appId,
            IDictionary<string, string> profileProperties)
        {
            if (profileProperties == null || !profileProperties.Any())
            {
                throw new ArgumentNullException($"Se requieren las propiedades de perfil para el usuario.");
            }

            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();
                foreach (KeyValuePair<string, string> kvp in profileProperties)
                {
                    using (SqlCommand command = new SqlCommand("[dbo].[SetUserProfileProperty]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("AppId", appId);
                        command.Parameters.AddWithValue("UserId", userId);
                        command.Parameters.AddWithValue("PropertyName", kvp.Key);
                        command.Parameters.AddWithValue("PropertyValue", kvp.Value);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Verifica si el secreto de una aplicación está encriptado.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        /// <returns><c>true</c> cuando el formato de secreto es encriptado; de lo contrario <c>false</c>.</returns>
        public bool AppSecretFormatIsEncrypted(string appKey)
        {
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();
                string commandText = @"
SELECT
    [AppId],
    [SecretFormat]
FROM [dbo].[Apps]
WHERE [AppKey] = @AppKey
";
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("AppKey", appKey);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        reader.Read();
                        int secretFormat = reader.HasRows ? reader.GetInt32(1) : 0;
                        return secretFormat == 1;
                    }
                }
            }
        }

        /// <summary>
        /// Se asegura que el token de autenticación generado por una aplicación haya expirado.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        public void EnsureExpireAppAuthToken(string appKey)
        {
            int appId = this.GetAppId(appKey);
            int randomDays = new Random().Next(2, 10);
            DateTimeOffset expiresAt = DateTimeOffset.UtcNow.AddDays(-randomDays);
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
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
        /// <param name="appKey">El identificador de la aplicación.</param>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        /// <param name="deviceId">El identificador del dispositivo.</param>
        public void EnsureExpireUserAuthToken(
            string appKey,
            string docType,
            string docNumber,
            string deviceId)
        {
            int userId = this.GetUserId(docType, docNumber);
            int appId = this.GetAppId(appKey);
            int randomDays = new Random().Next(2, 10);
            DateTimeOffset expiresAt = DateTimeOffset.UtcNow.AddDays(-randomDays);
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
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
        public void EnsureMismatchAppAuthToken(string appKey)
        {
            int appId = this.GetAppId(appKey);
            string randomToken = $"{Guid.NewGuid():N}{Guid.NewGuid():N}";
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
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
        /// <param name="appKey">El identificador de la aplicación.</param>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        /// <param name="deviceId">El identificador del dispositivo.</param>
        public void EnsureMismatchUserAuthToken(
            string appKey,
            string docType,
            string docNumber,
            string deviceId)
        {
            int userId = this.GetUserId(docType, docNumber);
            int appId = this.GetAppId(appKey);
            string randomToken = $"{Guid.NewGuid():N}{Guid.NewGuid():N}";
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
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
        /// Crea un usuario con un perfil predeterminado en el sistema; si el usuario ya existe, se asegura que no esté bloqueado y se sobreescriben los datos del perfil.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        /// <param name="pinNumber">Pin transaccional del usuario.</param>
        /// <param name="firstName">Nombres del usuario.</param>
        /// <param name="lastName">Apellidos del usuario.</param>
        /// <param name="secret">Clave de acceso del usuario.</param>
        /// <param name="email">Correo electrónico del usuario.</param>
        public void EnsureUserAndProfileInfo(
            string appKey,
            string docType,
            string docNumber,
            string pinNumber = null,
            string firstName = null,
            string lastName = null,
            string secret = null,
            string email = null)
        {
            this.EnsureUserInfo(docType, docNumber);
            int userId = this.GetUserId(docType, docNumber);
            int appId = this.GetAppId(appKey);
            var pinNumberInfo = new
            {
                PinNumber = pinNumber ?? "141414",
                PinFormat = "Clear",
                DeviceId = $@"{Environment.UserDomainName}\{Environment.UserName}",
                LastUpdateAt = DateTime.Now
            };
            Dictionary<string, string> profileProperties = new Dictionary<string, string>()
                                                              {
                                                                  { "FirstName", firstName ?? "Pepe" },
                                                                  { "LastName", lastName ?? "Cortisona" },
                                                                  { "Secret", secret ?? "colombia" },
                                                                  { "SecretFormat", "Clear" },
                                                                  { "Email", email ?? "pepe@cortisona.com" },
                                                                  { "PinMetadata", JsonConvert.SerializeObject(pinNumberInfo, Formatting.None) }
                                                              };
            this.AddUserProfileProperties(userId, appId, profileProperties);
        }

        /// <summary>
        /// Crea un usuario con un perfil predeterminado en el sistema; si el usuario ya existe, se asegura que no esté bloqueado y se sobreescriben los datos del perfil.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        /// <param name="profileProperties">Una colección de claves y valores que representa las propiedades del perfil del usuario.</param>
        public void EnsureUserAndProfileInfo(
            string appKey,
            string docType,
            string docNumber,
            IDictionary<string, string> profileProperties)
        {
            this.EnsureUserInfo(docType, docNumber);
            int userId = this.GetUserId(docType, docNumber);
            int appId = this.GetAppId(appKey);
            this.AddUserProfileProperties(userId, appId, profileProperties);
        }

        /// <summary>
        /// Crea un usuario en el sistema; si el usuario ya existe, se asegura que no esté bloqueado.
        /// </summary>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        public void EnsureUserInfo(string docType, string docNumber)
        {
            int userId = this.GetUserId(docType, docNumber);
            if (userId > 0)
            {
                this.EnsureUserIsNotLocked(docType, docNumber);
                return;
            }

            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
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
        /// Se asegura que el usuario este bloqueado.
        /// </summary>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        public void EnsureUserIsLocked(string docType, string docNumber)
        {
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
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
        public void EnsureUserIsNotLocked(string docType, string docNumber)
        {
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
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
        public int GetAppId(string appKey)
        {
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
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
                        return reader.HasRows ? reader.GetInt32(0) : 0;
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene el número máximo de intentos de autenticación inválidos permitidos, antes de bloquear a un usuario.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        /// <returns>Núméro máximo de intentos de autenticación inválidos permitidos</returns>
        public int GetAppMaxFailedPasswordAttempt(string appKey)
        {
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
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
                        return reader.HasRows ? reader.GetInt32(0) : 0;
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
        public int GetUserId(string docType, string docNumber)
        {
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();
                string commandText = @"
SELECT
    [UserId]
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
                        return reader.HasRows ? reader.GetInt32(0) : 0;
                    }
                }
            }
        }

        /// <summary>
        /// Elimina el token de autenticación emitido por una aplicación.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        public void RemoveAppAuthToken(string appKey)
        {
            int appId = this.GetAppId(appKey);
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
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
        /// <param name="appKey">El identificador de la aplicación.</param>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        /// <param name="deviceId">El identificador del dispositivo.</param>
        public void RemoveUserAuthToken(
            string appKey,
            string docType,
            string docNumber,
            string deviceId)
        {
            int appId = this.GetAppId(appKey);
            int userId = this.GetUserId(docType, docNumber);
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
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
        public void RemoveUserInfo(string docType, string docNumber)
        {
            int userId = this.GetUserId(docType, docNumber);
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
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
        /// Establece una clave en la configuración personalizadas de la aplicación.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        /// <param name="key">El nombre de la clave.</param>
        /// <param name="value">El valor para la clave.</param>
        public void SetAppSettingsKey(
            string appKey,
            string key,
            string value)
        {
            int appId = this.GetAppId(appKey);
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();
                string commandText = $@"
UPDATE [dbo].[Apps]
  SET [JsonSettings] = JSON_MODIFY([JsonSettings], '$.appSettings.""{key}""', '{value}')
WHERE[AppId] = @AppId;
";
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("AppId", appId);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Restaura el secreto de una aplicación.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        /// <param name="apiSecret">El secreto en claro para la aplicación.</param>
        public void UpdateApiSecret(string appKey, string apiSecret)
        {
            int appId = this.GetAppId(appKey);
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();
                string commandText = @"
UPDATE [dbo].[Apps]
  SET [Secret] = @Secret,
      [SecretFormat] = 0
 WHERE [AppId] = @AppId
";
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("AppId", appId);
                    command.Parameters.AddWithValue("Secret", apiSecret);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Asegura que una aplicación requiera (o no) cambio de su secreto.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        /// <param name="newValue"><see langword="true" /> cuando la aplicación requiera cambio de secreto; de lo contrario <see langword="false" />.</param>
        public void UpdateChangeSecret(string appKey, bool newValue)
        {
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();
                string commandText = @"
UPDATE [dbo].[Apps]
  SET [UpdateSecret] = @UpdateSecret
WHERE [AppKey] = @AppKey
";
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("AppKey", appKey);
                    command.Parameters.AddWithValue("UpdateSecret", newValue);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Habilita o inhabilita una aplicación en el sistema del servicio.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        /// <param name="enabled"><see langword="true" /> para habilitar la aplicación; de lo contrario <see langword="false" />.</param>
        public void UpdateEnabled(string appKey, bool enabled)
        {
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();
                string commandText = @"
UPDATE [dbo].[Apps]
  SET [Enabled] = @Enabled
WHERE [AppKey] = @AppKey
";
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("AppKey", appKey);
                    command.Parameters.AddWithValue("Enabled", enabled);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}