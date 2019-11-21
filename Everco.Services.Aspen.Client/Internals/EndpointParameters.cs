// -----------------------------------------------------------------------
// <copyright file="EndpointParameters.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-11-19 17:25 PM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Internals
{
    using System.Collections.Generic;

    /// <summary>
    /// Una colección que representa a los parámetros de un endpoint.
    /// </summary>
    public class EndpointParameters : Dictionary<string, object>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EndpointParameters" />
        /// </summary>
        public EndpointParameters() : base(new Dictionary<string, object>())
        {
        }

        /// <summary>
        /// Agrega el parámetro que representa el tipo de documento asociado a un usuario.
        /// </summary>
        /// <param name="docType">El valor del tipo de documento del usuario.</param>
        /// <returns>Instancia de <see cref="EndpointParameters"/> con la colección actual de parámetros.</returns>
        public EndpointParameters AddDocType(string docType)
        {
            Throw.IfNullOrEmpty(docType, nameof(docType));
            this.Add("@[DocType]", docType);
            return this;
        }

        /// <summary>
        /// Agrega el parámetro que representa el número de documento asociado a un usuario.
        /// </summary>
        /// <param name="docNumber">El valor del número de documento del usuario.</param>
        /// <returns>Instancia de <see cref="EndpointParameters"/> con la colección actual de parámetros.</returns>
        public EndpointParameters AddDocNumber(string docNumber)
        {
            Throw.IfNullOrEmpty(docNumber, nameof(docNumber));
            this.Add("@[DocNumber]", docNumber);
            return this;
        }

        /// <summary>
        /// Agrega el parámetro que representa el identificador de una cuenta.
        /// </summary>
        /// <param name="accountId">El valor del identificador de la cuenta.</param>
        /// <returns>Instancia de <see cref="EndpointParameters"/> con la colección actual de parámetros.</returns>
        public EndpointParameters AddAccountId(string accountId)
        {
            Throw.IfNullOrEmpty(accountId, nameof(accountId));
            this.Add("@[AccountId]", accountId);
            return this;
        }

        /// <summary>
        /// Agrega el parámetro que representa el tipo de cuenta.
        /// </summary>
        /// <param name="accountTypeId">El valor del tipo de cuenta.</param>
        /// <returns>Instancia de <see cref="EndpointParameters"/> con la colección actual de parámetros.</returns>
        public EndpointParameters AddAccountTypeId(string accountTypeId)
        {
            Throw.IfNullOrEmpty(accountTypeId, nameof(accountTypeId));
            this.Add("@[AccountTypeId]", accountTypeId);
            return this;
        }
    }
}