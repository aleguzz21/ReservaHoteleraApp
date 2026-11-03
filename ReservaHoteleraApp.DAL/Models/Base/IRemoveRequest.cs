using System;
using System.Collections.Generic;
using System.Text;

namespace ReservaHoteleraApp.DAL.Models.Base
{
    public interface IRemoveRequest
    {
        int Id { get; set; }
        int? ModifyUser { get; set; }
    }
}
