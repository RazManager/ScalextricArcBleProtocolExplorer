import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatDrawerMode } from '@angular/material/sidenav';
import { BreakpointState } from '@angular/cdk/layout';

import { CommonBusyService } from 'src/lib/common/common-busy.service';
import { CommonMenuService } from 'src/lib/components/common-menu/common-menu.service';
import { CommonBreakpointObserver } from 'src/lib/components/common-breakpointobserver/common-breakpointobserver.service';


@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    providers: [CommonBreakpointObserver]
})
export class AppComponent implements OnInit {
    public busy = false;
    public sidenavMode: MatDrawerMode = "side";
    public sidenavOpened: boolean = false;


    constructor(private readonly router: Router,
                private readonly menuService: CommonMenuService,
                private readonly breakpointObserver: CommonBreakpointObserver,
                public readonly busyService: CommonBusyService) {
    }


    public ngOnInit(): void {
        this.breakpointObserver
            .observeNarrow
            .subscribe({
                next: (state: BreakpointState) => {
                    this.sidenavMode = state.matches ? "over" : "side";
                    this.sidenavOpened = !state.matches;
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
    }


    public navigate(commands: any[]): void {
        if (this.breakpointObserver.isNarrow) {
            this.sidenavOpened = false;
        }
        this.router.navigate(commands);
    }
}