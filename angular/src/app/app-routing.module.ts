import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SystemResolver } from './views/system/system-resolver.service';
import { SystemComponent } from './views/system/system.component';

const routes: Routes = [
    {
        path: 'system',
        component: SystemComponent,
        resolve: {
            result: SystemResolver
        }
    }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
