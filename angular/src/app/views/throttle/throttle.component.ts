import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Data } from '@angular/router';
import { BreakpointState } from '@angular/cdk/layout';
import { MatSnackBar } from '@angular/material/snack-bar';

import { CommonBaseComponent } from 'src/lib/common/common-base.component';
import { CommonMenuService } from 'src/lib/components/common-menu/common-menu.service';
import { CommonBreakpointObserver } from 'src/lib/components/common-breakpointobserver/common-breakpointobserver.service';

import { ApiService } from '../api.service';
import { ThrottleDto } from './throttle.dto';
import { ThrottleObserversService } from './throttle-observers.service';


@Component({
    templateUrl: './throttle.component.html',
    providers: [ ThrottleObserversService ]

})
export class ThrottleComponent
        extends CommonBaseComponent
        implements OnInit {
    public dto!: ThrottleDto;
    public menuShow = false;


    constructor(snackBar: MatSnackBar,
                private readonly route: ActivatedRoute,
                private readonly menuService: CommonMenuService,
                private readonly breakpointObserver: CommonBreakpointObserver,
                private readonly observersService: ThrottleObserversService,
                private readonly apiService: ApiService) {
        super(snackBar);
    }


    public ngOnInit(): void {
        this.breakpointObserver
            .observeNarrow
            .subscribe((state: BreakpointState) => {
                this.menuShow = state.matches;
            });

        this.route.data.subscribe({
            next: (data: Data) => {
                this.dto = <ThrottleDto>data['result'];

                this.observersService
                .onChangedState
                .subscribe((dto: ThrottleDto) => {
                    console.log('ThrottleDto', dto);
                    this.dto = dto;
                });

                this.observersService.observe();
            }
        });
    }


    public menuToggle(): void {
        this.menuService.menuToogle();
    }
}