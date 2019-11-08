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
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Expone métodos para interactuar con la base de datos del servicio Aspen para fines de preparación de los escenarios de las pruebas automáticas.
    /// </summary>
    [SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "Keep calm and ignore")]
    public class SqlDataContext
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private static SqlDataContext @default;

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        /// Prevents a default instance of the <see cref="SqlDataContext"/> class from being created.
        /// </summary>
        private SqlDataContext()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{Environment.UserName}.json", true, true);
            IConfigurationRoot configuration = builder.Build();
            this.connectionString = configuration.GetConnectionString("Sql:Aspen");
        }

        /// <summary>
        /// Obtiene la instancia predeterminada.
        /// </summary>
        public static SqlDataContext Default => @default ?? (@default = new SqlDataContext());

        /// <summary>
        /// Crea un usuario en el sistema.
        /// </summary>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        public void AddUserInfo(string docType, string docNumber)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
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
        /// Crea un usuario con perfil en el sistema.
        /// </summary>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        /// <param name="appKey">El identificador de la aplicación.</param>
        /// <param name="profileProperties">Una colección de claves y valores que representa las propiedades del perfil del usuario.</param>
        public void AddUserInfo(string docType, string docNumber, string appKey, IDictionary<string, string> profileProperties)
        {
            this.AddUserInfo(docType, docNumber);
            int userId = this.GetUserId(docType, docNumber);
            int appId = this.GetAppId(appKey);
            this.AddUserProfileProperties(userId, appId, profileProperties);
        }

        /// <summary>
        /// Se asegura que el usuario este bloqueado.
        /// </summary>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        public void EnsureUserIsLocked(string docType, string docNumber)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
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
                    command.Parameters.AddWithValue("LastLockoutDate", DateTimeOffset.Now);
                    command.Parameters.AddWithValue("FailedPasswordAttemptWindowStart", DateTimeOffset.Now);
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
            using (SqlConnection connection = new SqlConnection(this.connectionString))
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
            using (SqlConnection connection = new SqlConnection(this.connectionString))
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
                        return reader.GetInt32(reader.GetOrdinal("AppId"));
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
            using (SqlConnection connection = new SqlConnection(this.connectionString))
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
                        return reader.GetInt32(reader.GetOrdinal("MaxFailedPasswordAttempt"));
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
            using (SqlConnection connection = new SqlConnection(this.connectionString))
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
                        return reader.GetInt32(reader.GetOrdinal("UserId"));
                    }
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
            using (SqlConnection connection = new SqlConnection(this.connectionString))
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
        private void AddUserProfileProperties(int userId, int appId, IDictionary<string, string> profileProperties)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
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