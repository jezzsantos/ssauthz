﻿//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by a tool.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Services.MessageContracts
{
    /// <summary>
    /// Defines the response for the <see cref="CreateClientApplication"/> request.
    /// </summary>
    public partial class CreateClientApplicationResponse
    {
        /// <summary>
        /// Gets or sets the <see cref="ServiceStack.ResponseStatus"/> of the response.
        /// </summary>
        public ServiceStack.ResponseStatus ResponseStatus { get; set; }

        /// <summary>
        /// Gets or sets the clientapplication
        /// </summary>
        public DataContracts.ClientApplication ClientApplication { get; set; }

    }
}