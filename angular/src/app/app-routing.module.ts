import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { CommandResolver } from './views/command/command-resolver.service';
import { CommandComponent } from './views/command/command.component';
import { GattCharacteristicResolver } from './views/gatt-characteristic/gatt-characteristic-resolver.service';
import { GattCharacteristicComponent } from './views/gatt-characteristic/gatt-characteristic.component';
import { LogResolver } from './views/log/log-resolver.service';
import { LogComponent } from './views/log/log.component';
import { SlotResolver } from './views/slot/slot-resolver.service';
import { SlotComponent } from './views/slot/slot.component';
import { SystemInformationResolver } from './views/system-information/system-information-resolver.service';
import { SystemInformationComponent } from './views/system-information/system-information.component';
import { ThrottleResolver } from './views/throttle/throttle-resolver.service';
import { ThrottleComponent } from './views/throttle/throttle.component';
import { ThrottleProfileResolver } from './views/throttle-profile/throttle-profile-resolver.service';
import { ThrottleProfileComponent } from './views/throttle-profile/throttle-profile.component';

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
        component: CommandComponent,
        resolve: {
            result: CommandResolver
        }
    },
    {
        path: 'gatt-characteristic',
        component: GattCharacteristicComponent,
        resolve: {
            result: GattCharacteristicResolver
        }
    },
    {
        path: 'log',
        component: LogComponent,
        resolve: {
            result: LogResolver
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
    },
    {
        path: 'throttle-profile',
        component: ThrottleProfileComponent,
        resolve: {
            result: ThrottleProfileResolver
        }
    }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
