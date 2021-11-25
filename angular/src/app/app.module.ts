import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card'; 
import { MatCheckboxModule } from '@angular/material/checkbox'; 
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatSliderModule } from '@angular/material/slider';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatToolbarModule } from '@angular/material/toolbar';
import { FlexLayoutModule } from '@angular/flex-layout';

import { CommonBusyService } from 'src/lib/common/common-busy.service';
import { CommonMenuService } from 'src/lib/components/common-menu/common-menu.service';
import { CommonToolbarComponent } from 'src/lib/components/common-toolbar/common-toolbar.component';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ApiService } from './views/api.service';
import { DeviceResolver } from './views/device/device-resolver.service';
import { DeviceComponent } from './views/device/device.component';
import { ThrottleResolver } from './views/throttle/throttle-resolver.service';
import { ThrottleComponent } from './views/throttle/throttle.component';
import { SystemResolver } from './views/system/system-resolver.service';
import { SystemComponent } from './views/system/system.component';


@NgModule({
    declarations: [
        AppComponent,
        CommonToolbarComponent,
        DeviceComponent,
        ThrottleComponent,
        SystemComponent
    ],
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        HttpClientModule,
        MatButtonModule,
        MatCardModule,
        MatCheckboxModule,
        MatIconModule,
        MatListModule,
        MatProgressSpinnerModule,
        MatSidenavModule,
        MatSliderModule,
        MatSnackBarModule,
        MatToolbarModule,
        FlexLayoutModule,
        AppRoutingModule
    ],
    providers: [
        CommonMenuService,
        CommonBusyService,
        ApiService,
        DeviceResolver,
        ThrottleResolver,
        SystemResolver
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
