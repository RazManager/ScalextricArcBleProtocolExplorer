import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { CommandComponent } from './views/command/command.component';
import { DeviceInformationResolver } from './views/device-information/device-information-resolver.service';
import { DeviceInformationComponent } from './views/device-information/device-information.component';
import { SlotResolver } from './views/slot/slot-resolver.service';
import { SlotComponent } from './views/slot/slot.component';
import { SystemInformationResolver } from './views/system-information/system-information-resolver.service';
import { SystemInformationComponent } from './views/system-information/system-information.component';
import { ThrottleResolver } from './views/throttle/throttle-resolver.service';
import { ThrottleComponent } from './views/throttle/throttle.component';

const routes: Routes = [
    {
        path: '',
        component: SystemInformationComponent,
        resolve: {
            result: SystemInformationResolver
        }
    },
    {
        path: 'command',
        component: CommandComponent
    },
    {
        path: 'device',
        component: DeviceInformationComponent,
        resolve: {
            result: DeviceInformationResolver
        }
    },
    {
        path: 'slot',
        component: SlotComponent,
        resolve: {
            result: SlotResolver
        }
    },
    {
        path: 'system',
        component: SystemInformationComponent,
        resolve: {
            result: SystemInformationResolver
        }
    },
    {
        path: 'throttle',
        component: ThrottleComponent,
        resolve: {
            result: ThrottleResolver
        }
    }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
