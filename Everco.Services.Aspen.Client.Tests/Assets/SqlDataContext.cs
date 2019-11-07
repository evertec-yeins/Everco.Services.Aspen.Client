// -----------------------------------------------------------------------
// <copyright file="SqlDataContext.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-11-07 16:45 PM</date>
// -----------------------------------------------------------------------
// ReSharper disable ConvertToNullCoalescingCompoundAssignment
namespace Everco.Services.Aspen.Client.Tests.Assets
{
    using System;
    using System.Data.SqlClient;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Implementa un generador de épocas o marcas de tiempo Unix (número de segundos que han transcurrido desde 1970-01-01T00:00:00Z)
    /// </summary>
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
                using (SqlCommand updateCommand = new SqlCommand(commandText, connection))
                {
                    updateCommand.Parameters.AddWithValue("DocType", docType);
                    updateCommand.Parameters.AddWithValue("DocNumber", docNumber);
                    updateCommand.ExecuteNonQuery();
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
                using (SqlCommand updateCommand = new SqlCommand(commandText, connection))
                {
                    updateCommand.Parameters.AddWithValue("DocType", docType);
                    updateCommand.Parameters.AddWithValue("DocNumber", docNumber);
                    updateCommand.Parameters.AddWithValue("LastLockoutDate", DateTimeOffset.Now);
                    updateCommand.Parameters.AddWithValue("FailedPasswordAttemptWindowStart", DateTimeOffset.Now);
                    updateCommand.Parameters.AddWithValue("FailedPasswordAttemptCount", 10);
                    updateCommand.Parameters.AddWithValue("IsLockedOut", true);
                    updateCommand.ExecuteNonQuery();
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
		[IsLockedOut] = @IsLockedOut
	WHERE [DocType] = @DocType AND [DocNumber] = @DocNumber
";
                using (SqlCommand updateCommand = new SqlCommand(commandText, connection))
                {
                    updateCommand.Parameters.AddWithValue("DocType", docType);
                    updateCommand.Parameters.AddWithValue("DocNumber", docNumber);
                    updateCommand.Parameters.AddWithValue("LastLockoutDate", DBNull.Value);
                    updateCommand.Parameters.AddWithValue("FailedPasswordAttemptCount", 0);
                    updateCommand.Parameters.AddWithValue("FailedPasswordAttemptWindowStart", DBNull.Value);
                    updateCommand.Parameters.AddWithValue("IsLockedOut", false);
                    updateCommand.ExecuteNonQuery();
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
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                string commandText = @"
DELETE FROM [dbo].[Users]
      WHERE [DocType] = @DocType AND [DocNumber] = @DocNumber
";
                using (SqlCommand updateCommand = new SqlCommand(commandText, connection))
                {
                    updateCommand.Parameters.AddWithValue("DocType", docType);
                    updateCommand.Parameters.AddWithValue("DocNumber", docNumber);
                    updateCommand.ExecuteNonQuery();
                }
            }
        }
    }
}