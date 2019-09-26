// -----------------------------------------------------------------------
// <copyright file="HeaderValueBehavior.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 11:48 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests.Assets
{
    /// <summary>
    /// 
    /// </summary>
    public enum HeaderValueBehavior
    {
        Missing = 0,

        Null = 1,

        Empty = 2,

        WhiteSpaces = 3,

        UnexpectedFormat = 4,

        MaxLengthExceeded = 5,

        MinLengthRequired = 6
    }
}
