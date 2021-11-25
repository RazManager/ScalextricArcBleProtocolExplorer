import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { DeviceResolver } from './views/device/device-resolver.service';
import { DeviceComponent } from './views/device/device.component';
import { ThrottleResolver } from './views/throttle/throttle-resolver.service';
import { ThrottleComponent } from './views/throttle/throttle.component';
import { SystemResolver } from './views/system/system-resolver.service';
import { SystemComponent } from './views/system/system.component';

const routes: Routes = [
    {
        path: 'system',
        component: SystemComponent,
        resolve: {
            result: SystemResolver
        }
    },
    {
        path: 'device',
        component: DeviceComponent,
        resolve: {
            result: DeviceResolver
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
