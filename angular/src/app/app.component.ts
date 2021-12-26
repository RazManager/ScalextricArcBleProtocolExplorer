import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatDrawerMode } from '@angular/material/sidenav';
import { BreakpointState } from '@angular/cdk/layout';

import { CommonBusyService } from 'src/lib/common/common-busy.service';
import { CommonMenuService } from 'src/lib/components/common-menu/common-menu.service';
import { CommonToolbarService } from 'src/lib/components/common-toolbar/common-toolbar.service';
import { CommonBreakpointObserver } from 'src/lib/components/common-breakpointobserver/common-breakpointobserver.service';

import { ApiService } from './views/api.service';
import { CommandDto, CommandType } from './views/command/command.dto';
import { CommandObserversService } from './views/command/command-observers.service';
import { BluetoothConnectionStateType, ConnectionDto } from './views/connection/connection.dto';
import { ConnectionObserversService } from './views/connection/connection-observers.service';


@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    providers: [CommonBreakpointObserver, ConnectionObserversService, CommandObserversService]
})
export class AppComponent implements OnInit {
    public busy = false;
    public sidenavMode: MatDrawerMode = "side";
    public sidenavOpened: boolean = false;
    public menuShow = false;
    public command: CommandDto | undefined;
    public connection: ConnectionDto | undefined;


    constructor(private readonly router: Router,
                private readonly menuService: CommonMenuService,
                public readonly toolbarService: CommonToolbarService,
                private readonly breakpointObserver: CommonBreakpointObserver,
                public readonly busyService: CommonBusyService,
                private readonly apiService: ApiService,
                private readonly commandObserversService: CommandObserversService,
                private readonly connectionObserversService: ConnectionObserversService) {
    }


    public ngOnInit(): void {
        this.breakpointObserver
            .observeNarrow
            .subscribe({
                next: (state: BreakpointState) => {
                    this.sidenavMode = state.matches ? "over" : "side";
                    this.sidenavOpened = !state.matches;
                    this.menuShow = state.matches;
                }
            });

        this.menuService.onMenuToggle
            .subscribe({
                next: () => {
                    this.sidenavOpened = !this.sidenavOpened;
                }                
            });

        this.busyService.onBusyChanged
            .subscribe({
                next: (busy: boolean) => this.busy = busy
            });

        this.connectionObserversService
            .onChangedState
            .subscribe((dto: ConnectionDto) => {
                this.connection = dto;
            });

        this.connectionObserversService.observe();

        this.commandObserversService
            .onChangedState
            .subscribe((dto: CommandDto) => {
                this.command = dto;
            });

        this.commandObserversService.observe();

        this.apiService.getConnection()
            .subscribe((dto: ConnectionDto) => 
            {
                this.connection = dto;
            });


        this.apiService.getCommand()
            .subscribe((dto: CommandDto) => 
            {
                this.command = dto;
            });
    }


    public navigate(commands: any[]): void {
        if (this.breakpointObserver.isNarrow) {
            this.sidenavOpened = false;
        }
        this.router.navigate(commands);
    }



    public menuToggle(): void {
        this.menuService.menuToogle();
    }


    public get powerOff(): boolean {
        if (!this.command || !this.connection || this.connection.bluetoothConnectionState !== BluetoothConnectionStateType.Initialized) {
            return false;
        }

        if (this.connection.modelNumber === "Scalextric ARC ONE") {
            return false;
        }
        else {
            switch (this.command.command) {
                case CommandType.NoPowerTimerStopped:
                    return true;
                case CommandType.NoPowerTimerTicking:
                    return false;
                case CommandType.PowerOnRaceTrigger:
                    return false;
                case CommandType.PowerOnRacing:
                    return false;
                case CommandType.PowerOnTimerHalt:
                    return true;
                case CommandType.NoPowerRebootPic18:
                    return false;                                                     
            }
        }
    }


    public get powerOn(): boolean {
        if (!this.command || !this.connection || this.connection.bluetoothConnectionState !== BluetoothConnectionStateType.Initialized) {
            return false;
        }

        if (this.connection.modelNumber === "Scalextric ARC ONE") {
            return true;
        }
        else {
            switch (this.command.command) {
                case CommandType.NoPowerTimerStopped:
                    return false;
                case CommandType.NoPowerTimerTicking:
                    return true;
                case CommandType.PowerOnRaceTrigger:
                    return true;
                case CommandType.PowerOnRacing:
                    return true;
                case CommandType.PowerOnTimerHalt:
                    return false;
                case CommandType.NoPowerRebootPic18:
                    return true;                                                     
            }
        }
    }


    public get timerOff(): boolean {
        if (!this.command || !this.connection || this.connection.bluetoothConnectionState !== BluetoothConnectionStateType.Initialized) {
            return false;
        }

        switch (this.command.command) {
            case CommandType.NoPowerTimerStopped:
                return true;
            case CommandType.NoPowerTimerTicking:
                return true;
            case CommandType.PowerOnRaceTrigger:
                return true;
            case CommandType.PowerOnRacing:
                return false;
            case CommandType.PowerOnTimerHalt:
                return true;
            case CommandType.NoPowerRebootPic18:
                return false;                                                     
        }
    }


    public get timerOn(): boolean {
        if (!this.command || !this.connection || this.connection.bluetoothConnectionState !== BluetoothConnectionStateType.Initialized) {
            return false;
        }

        switch (this.command.command) {
            case CommandType.NoPowerTimerStopped:
                return false;
            case CommandType.NoPowerTimerTicking:
                return false;
            case CommandType.PowerOnRaceTrigger:
                return false;
            case CommandType.PowerOnRacing:
                return true;
            case CommandType.PowerOnTimerHalt:
                return false;
            case CommandType.NoPowerRebootPic18:
                return true;                                                     
        }
    }
}