﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fusion.Framework.Exchange.Models
{
    [Serializable]
    public enum TipoConflito
    {
        //     There is a conflict with an indicidual attendee.
        IndividualAttendeeConflict = 0,
        //     There is a conflict with at least one member of a group.
        GroupConflict = 1,        
        //     There is a conflict with at least one member of a group, but the group was
        //     too big for detailed information to be returned.
        GroupTooBigConflict = 2,
        //     There is a conflict with an unresolvable attendee or an attendee that is
        //     not a user, group, or contact.
        UnknownAttendeeConflict = 3,
    }
}
