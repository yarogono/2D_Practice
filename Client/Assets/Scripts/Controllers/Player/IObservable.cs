﻿using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Controllers.Player
{
    interface ISyncObservable
    {

        void OnPhotonSerializeView(IMessage mesage);
    }
}
