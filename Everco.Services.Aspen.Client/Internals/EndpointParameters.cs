// -----------------------------------------------------------------------
// <copyright file="EndpointParameters.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-11-19 17:25 PM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Internals
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Una colección que representa a los parámetros de un endpoint.
    /// </summary>
    internal class EndpointParameters : Dictionary<string, object>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EndpointParameters" />
        /// </summary>
        internal EndpointParameters() : base(new Dictionary<string, object>())
        {
        }

        /// <summary>
        /// Agrega a la colección el parámetro que representa el nombre o alias que identifica a una cuenta.
        /// </summary>
        /// <param name="accountAlias">El nombre o alias que identifica a una cuenta.</param>
        /// <returns>Instancia de <see cref="EndpointParameters"/> con la colección actual de parámetros.</returns>
        internal EndpointParameters AddAccountAlias(string accountAlias)
        {
            Throw.IfNullOrEmpty(accountAlias, nameof(accountAlias));
            this.Add("@[Alias]", accountAlias);
            return this;
        }

        /// <summary>
        /// Agrega el parámetro que representa el identificador de una cuenta.
        /// </summary>
        /// <param name="accountId">El valor del identificador de la cuenta.</param>
        /// <returns>Instancia de <see cref="EndpointParameters"/> con la colección actual de parámetros.</returns>
        internal EndpointParameters AddAccountId(string accountId)
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
        internal EndpointParameters AddAccountTypeId(string accountTypeId)
        {
            Throw.IfNullOrEmpty(accountTypeId, nameof(accountTypeId));
            this.Add("@[AccountTypeId]", accountTypeId);
            return this;
        }

        /// <summary>
        /// Agrega a la colección el parámetro que representa el número de documento del titular de una cuenta.
        /// </summary>
        /// <param name="cardHolderDocNumber">El valor del número de documento del titular.</param>
        /// <returns>Instancia de <see cref="EndpointParameters"/> con la colección actual de parámetros.</returns>
        internal EndpointParameters AddCardHolderDocNumber(string cardHolderDocNumber)
        {
            Throw.IfNullOrEmpty(cardHolderDocNumber, nameof(cardHolderDocNumber));
            this.Add("@[CardHolderDocNumber]", cardHolderDocNumber);
            return this;
        }

        /// <summary>
        /// Agrega a la colección el parámetro que representa el tipo de documento del titular de una cuenta.
        /// </summary>
        /// <param name="cardHolderDocType">El valor del tipo de documento del titular.</param>
        /// <returns>Instancia de <see cref="EndpointParameters"/> con la colección actual de parámetros.</returns>
        internal EndpointParameters AddCardHolderDocType(string cardHolderDocType)
        {
            Throw.IfNullOrEmpty(cardHolderDocType, nameof(cardHolderDocType));
            this.Add("@[CardHolderDocType]", cardHolderDocType);
            return this;
        }

        /// <summary>
        /// Agrega el parámetro que representa el identificador del canal por el que se registró un usuario.
        /// </summary>
        /// <param name="channelId">El valor del identificador del canal.</param>
        /// <returns>Instancia de <see cref="EndpointParameters"/> con la colección actual de parámetros.</returns>
        internal EndpointParameters AddChannelId(string channelId)
        {
            Throw.IfNullOrEmpty(channelId, nameof(channelId));
            this.Add("@[ChannelId]", channelId);
            return this;
        }

        /// <summary>
        /// Agrega el parámetro que representa el número de documento asociado a un usuario.
        /// </summary>
        /// <param name="docNumber">El valor del número de documento del usuario.</param>
        /// <returns>Instancia de <see cref="EndpointParameters"/> con la colección actual de parámetros.</returns>
        internal EndpointParameters AddDocNumber(string docNumber)
        {
            Throw.IfNullOrEmpty(docNumber, nameof(docNumber));
            this.Add("@[DocNumber]", docNumber);
            return this;
        }

        /// <summary>
        /// Agrega el parámetro que representa el tipo de documento asociado a un usuario.
        /// </summary>
        /// <param name="docType">El valor del tipo de documento del usuario.</param>
        /// <returns>Instancia de <see cref="EndpointParameters"/> con la colección actual de parámetros.</returns>
        internal EndpointParameters AddDocType(string docType)
        {
            Throw.IfNullOrEmpty(docType, nameof(docType));
            this.Add("@[DocType]", docType);
            return this;
        }

        /// <summary>
        /// Agrega el parámetro que representa al alias de un usuario.
        /// </summary>
        /// <param name="enrollmentAlias">El valor del alias.</param>
        /// <returns>Instancia de <see cref="EndpointParameters"/> con la colección actual de parámetros.</returns>
        internal EndpointParameters AddEnrollmentAlias(string enrollmentAlias)
        {
            Throw.IfNullOrEmpty(enrollmentAlias, nameof(enrollmentAlias));
            this.Add("@[EnrollmentAlias]", enrollmentAlias);
            return this;
        }

        /// <summary>
        /// Agrega a la colección el parámetro que representa el número de documento de un usuario.
        /// </summary>
        /// <param name="ownerDocNumber">El valor del número de documento del usuario.</param>
        /// <returns>Instancia de <see cref="EndpointParameters"/> con la colección actual de parámetros.</returns>
        internal EndpointParameters AddOwnerDocNumber(string ownerDocNumber)
        {
            Throw.IfNullOrEmpty(ownerDocNumber, nameof(ownerDocNumber));
            this.Add("@[OwnerDocNumber]", ownerDocNumber);
            return this;
        }

        /// <summary>
        /// Agrega a la colección el parámetro que representa el tipo de documento de un usuario.
        /// </summary>
        /// <param name="ownerDocType">El valor del tipo de documento del usuario.</param>
        /// <returns>Instancia de <see cref="EndpointParameters"/> con la colección actual de parámetros.</returns>
        internal EndpointParameters AddOwnerDocType(string ownerDocType)
        {
            Throw.IfNullOrEmpty(ownerDocType, nameof(ownerDocType));
            this.Add("@[OwnerDocType]", ownerDocType);
            return this;
        }
    }
}