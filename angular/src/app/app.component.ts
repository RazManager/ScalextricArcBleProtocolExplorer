import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatDrawerMode } from '@angular/material/sidenav';
import { BreakpointState } from '@angular/cdk/layout';

import { CommonBusyService } from 'src/lib/common/common-busy.service';
import { CommonMenuService } from 'src/lib/components/common-menu/common-menu.service';
import { CommonToolbarService } from 'src/lib/components/common-toolbar/common-toolbar.service';
import { CommonBreakpointObserver } from 'src/lib/components/common-breakpointobserver/common-breakpointobserver.service';

import { ApiService } from './views/api.service';
import { CommandDto } from './views/command/command.dto';
import { CommandService } from './views/command/command.service';
import { CommandObserversService } from './views/command/command-observers.service';
import { ConnectionDto } from './views/connection/connection.dto';
import { ConnectionService } from './views/connection/connection.service';
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


    constructor(private readonly router: Router,
                private readonly menuService: CommonMenuService,
                public readonly toolbarService: CommonToolbarService,
                private readonly breakpointObserver: CommonBreakpointObserver,
                public readonly busyService: CommonBusyService,
                private readonly apiService: ApiService,
                public readonly commandService: CommandService,
                private readonly commandObserversService: CommandObserversService,
                public readonly connectionService: ConnectionService,
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
                console.log('ConnectionDto', dto);
                this.connectionService.dto = dto;
            });

        this.connectionObserversService.observe();

        this.commandObserversService
            .onChangedState
            .subscribe((dto: CommandDto) => {
                console.log('CommandDto', dto);
                this.commandService.dto = dto;
            });

        this.commandObserversService.observe();

        this.apiService.getConnection()
            .subscribe((dto: ConnectionDto) => 
            {
                this.connectionService.dto = dto;
            });


        this.apiService.getCommand()
            .subscribe((dto: CommandDto) => 
            {
                this.commandService.dto = dto;
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
}