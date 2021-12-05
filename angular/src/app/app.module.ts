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
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs'; 
import { MatToolbarModule } from '@angular/material/toolbar';
import { FlexLayoutModule } from '@angular/flex-layout';

import { CommonBusyService } from 'src/lib/common/common-busy.service';
import { CommonMenuService } from 'src/lib/components/common-menu/common-menu.service';
import { CommonToolbarService } from 'src/lib/components/common-toolbar/common-toolbar.service';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ApiService } from './views/api.service';
import { CarIdResolver } from './views/car-id/car-id-resolver.service';
import { CarIdComponent } from './views/car-id/car-id.component';
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
import { TrackResolver } from './views/track/track-resolver.service';
import { TrackComponent } from './views/track/track.component';


@NgModule({
    declarations: [
        AppComponent,
        CarIdComponent,
        CommandComponent,
        GattCharacteristicComponent,
        LogComponent,
        SlotComponent,
        SystemInformationComponent,
        ThrottleComponent,
        ThrottleProfileComponent,
        TrackComponent
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
        MatTableModule,
        MatTabsModule,
        MatToolbarModule,
        FlexLayoutModule,
        AppRoutingModule
    ],
    providers: [
        CommonBusyService,
        CommonMenuService,
        CommonToolbarService,
        ApiService,
        CarIdResolver,
        CommandResolver,
        GattCharacteristicResolver,
        LogResolver,
        SlotResolver,
        SystemInformationResolver,
        ThrottleResolver,
        ThrottleProfileResolver,
        TrackResolver
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
