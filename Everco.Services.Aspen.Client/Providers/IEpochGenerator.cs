// -----------------------------------------------------------------------
// <copyright file="IEpochGenerator.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-23 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Providers
{
    /// <summary>
    /// Define los métodos para un generador de épocas Unix o marcas de tiempo Unix (número de segundos que han transcurrido desde 1970-01-01T00:00:00Z)
    /// </summary>
    /// <remarks>
    /// Se utiliza como un punto de referencia a partir del cual se mide el tiempo con el fin de omitir ambigüedades,
    /// debido a la gran variedad de unidades de tiempo empleadas en sistemas informáticos.
    /// </remarks>
    public interface IEpochGenerator
    {
        /// <summary>
        /// Obtiene el número de segundos que han transcurrido desde 1970-01-01T00:00:00Z
        /// </summary>
        /// <returns>Marca de tiempo Unix, expresado como el número de segundos que han transcurrido desde 1970-01-01T00:00:00Z</returns>
        double GetSeconds();
    }
}