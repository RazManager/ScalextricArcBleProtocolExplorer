import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card'; 
import { MatCheckboxModule } from '@angular/material/checkbox'; 
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatRadioModule } from '@angular/material/radio';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatSliderModule } from '@angular/material/slider';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatToolbarModule } from '@angular/material/toolbar';
import { FlexLayoutModule } from '@angular/flex-layout';

import { CommonBusyService } from 'src/lib/common/common-busy.service';
import { CommonMenuService } from 'src/lib/components/common-menu/common-menu.service';
import { CommonToolbarService } from 'src/lib/components/common-toolbar/common-toolbar.service';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ApiService } from './views/api.service';
import { CommandService } from './views/command/command.service';
import { CommandComponent } from './views/command/command.component';
import { ConnectionService } from './views/connection/connection.service';
import { DeviceInformationResolver } from './views/device-information/device-information-resolver.service';
import { DeviceInformationComponent } from './views/device-information/device-information.component';
import { SlotResolver } from './views/slot/slot-resolver.service';
import { SlotComponent } from './views/slot/slot.component';
import { SystemInformationResolver } from './views/system-information/system-information-resolver.service';
import { SystemInformationComponent } from './views/system-information/system-information.component';
import { ThrottleResolver } from './views/throttle/throttle-resolver.service';
import { ThrottleComponent } from './views/throttle/throttle.component';


@NgModule({
    declarations: [
        AppComponent,
        CommandComponent,
        DeviceInformationComponent,
        SlotComponent,
        SystemInformationComponent,
        ThrottleComponent
    ],
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        HttpClientModule,
        FormsModule,
        MatButtonModule,
        MatCardModule,
        MatCheckboxModule,
        MatIconModule,
        MatListModule,
        MatProgressSpinnerModule,
        MatRadioModule,
        MatSidenavModule,
        MatSliderModule,
        MatSnackBarModule,
        MatToolbarModule,
        FlexLayoutModule,
        AppRoutingModule
    ],
    providers: [
        CommonBusyService,
        CommonMenuService,
        CommonToolbarService,
        ApiService,
        CommandService,
        ConnectionService,
        DeviceInformationResolver,
        SlotResolver,
        SystemInformationResolver,
        ThrottleResolver
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
