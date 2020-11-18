// -----------------------------------------------------------------------
// <copyright file="IDatabaseHelper.cs" company="Processa">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-11-26 09:41 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests.Data
{
    using System.Collections.Generic;

    /// <summary>
    /// Define las operaciones necesarias para establecer y/o leer información de la base de datos del servicio.
    /// </summary>
    public interface IDatabaseHelper
    {
        /// <summary>
        /// Agrega las propiedades del perfil a un usuario.
        /// </summary>
        /// <param name="userId">El identificador del usuario en el sistema.</param>
        /// <param name="appId">El identificador de la aplicación en el sistema.</param>
        /// <param name="profileProperties">Una colección de claves y valores que representa las propiedades del perfil del usuario.</param>
        void AddUserProfileProperties(
            int userId,
            int appId,
            IDictionary<string, string> profileProperties);

        /// <summary>
        /// Verifica si el secreto de una aplicación está encriptado.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        /// <returns><c>true</c> cuando el formato de secreto es encriptado; de lo contrario <c>false</c>.</returns>
        bool AppSecretFormatIsEncrypted(string appKey);

        /// <summary>
        /// Se asegura que el token de autenticación generado por una aplicación haya expirado.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        void EnsureExpireAppAuthToken(string appKey);

        /// <summary>
        /// Se asegura que el token o clave transaccional de un usuario haya expirado.
        /// </summary>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        /// <param name="appKey">El identificador de la aplicación.</param>
        void EnsureExpireToken(
            string docType,
            string docNumber,
            string appKey);

        /// <summary>
        /// Se asegura que el token de autenticación de un usuario haya expirado.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        /// <param name="deviceId">El identificador del dispositivo.</param>
        void EnsureExpireUserAuthToken(
            string appKey,
            string docType,
            string docNumber,
            string deviceId);

        /// <summary>
        /// Se asegura de alterar el token de autenticación de una aplicación.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        void EnsureMismatchAppAuthToken(string appKey);

        /// <summary>
        /// Se asegura de alterar el token de autenticación de un usuario.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        /// <param name="deviceId">El identificador del dispositivo.</param>
        void EnsureMismatchUserAuthToken(
            string appKey,
            string docType,
            string docNumber,
            string deviceId);

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
        void EnsureUserAndProfileInfo(
            string appKey,
            string docType,
            string docNumber,
            string pinNumber = null,
            string firstName = null,
            string lastName = null,
            string secret = null,
            string email = null);

        /// <summary>
        /// Crea un usuario con un perfil predeterminado en el sistema; si el usuario ya existe, se asegura que no esté bloqueado y se sobreescriben los datos del perfil.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        /// <param name="profileProperties">Una colección de claves y valores que representa las propiedades del perfil del usuario.</param>
        void EnsureUserAndProfileInfo(
            string appKey,
            string docType,
            string docNumber,
            IDictionary<string, string> profileProperties);

        /// <summary>
        /// Crea un usuario en el sistema; si el usuario ya existe, se asegura que no esté bloqueado.
        /// </summary>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        void EnsureUserInfo(string docType, string docNumber);

        /// <summary>
        /// Se asegura que el usuario este bloqueado.
        /// </summary>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        void EnsureUserIsLocked(string docType, string docNumber);

        /// <summary>
        /// Se asegura que el usuario no este bloqueado.
        /// </summary>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        void EnsureUserIsNotLocked(string docType, string docNumber);

        /// <summary>
        /// Obtiene el identificador de la aplicación en el sistema.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        /// <returns>El identificador de la aplicación en el sistema.</returns>
        int GetAppId(string appKey);

        /// <summary>
        /// Obtiene el número máximo de intentos de autenticación inválidos permitidos, antes de bloquear a un usuario.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        /// <returns>Núméro máximo de intentos de autenticación inválidos permitidos</returns>
        int GetAppMaxFailedPasswordAttempt(string appKey);

        /// <summary>
        /// Obtiene el identificador de un usuario en el sistema.
        /// </summary>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        /// <returns>El identificador del usuario en el sistema.</returns>
        int GetUserId(string docType, string docNumber);

        /// <summary>
        /// Elimina el token de autenticación emitido por una aplicación.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        void RemoveAppAuthToken(string appKey);

        /// <summary>
        /// Elimina el token de autenticación emitido por un usuario.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        /// <param name="deviceId">El identificador del dispositivo.</param>
        void RemoveUserAuthToken(
            string appKey,
            string docType,
            string docNumber,
            string deviceId);

        /// <summary>
        /// Remueve a un usuario del sistema.
        /// </summary>
        /// <param name="docType">Tipo de documento del usuario.</param>
        /// <param name="docNumber">Número de documento del usuario.</param>
        void RemoveUserInfo(string docType, string docNumber);

        /// <summary>
        /// Establece una clave en la configuración personalizadas de la aplicación.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        /// <param name="key">El nombre de la clave.</param>
        /// <param name="value">El valor para la clave.</param>
        void SetAppSettingsKey(string appKey, string key, string value);

        /// <summary>
        /// Actualiza el secreto de una aplicación.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        /// <param name="apiSecret">El secreto en claro para la aplicación.</param>
        void UpdateApiSecret(string appKey, string apiSecret);

        /// <summary>
        /// Asegura que una aplicación requiera (o no) cambio de su secreto.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        /// <param name="newValue"><see langword="true" /> cuando la aplicación requiera cambio de secreto; de lo contrario <see langword="false" />.</param>
        void UpdateChangeSecret(string appKey, bool newValue);

        /// <summary>
        /// Habilita o inhabilita una aplicación en el sistema del servicio.
        /// </summary>
        /// <param name="appKey">El identificador de la aplicación.</param>
        /// <param name="enabled"><see langword="true" /> para habilitar la aplicación; de lo contrario <see langword="false" />.</param>
        void UpdateEnabled(string appKey, bool enabled);
    }
}