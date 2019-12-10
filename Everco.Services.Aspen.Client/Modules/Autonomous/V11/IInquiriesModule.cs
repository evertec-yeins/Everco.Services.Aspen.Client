// -----------------------------------------------------------------------
// <copyright file="IInquiriesModule.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-11-19 17:25 PM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Modules.Autonomous.V11
{
    using Everco.Services.Aspen.Entities;

    /// <summary>
    /// Define las consultas soportadas de los productos financieros para una aplicación con alcance de autónoma.
    /// </summary>
    public interface IInquiriesModule : IAccountInquiries<AccountResultInfo>,
                                        IBalanceInquiries<BalanceResultInfo>,
                                        IStatementInquiries<MiniStatementResultInfo>
    {
    }
}