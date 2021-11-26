import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Data } from '@angular/router';
import { BreakpointState } from '@angular/cdk/layout';
import { MatSnackBar } from '@angular/material/snack-bar';

import { CommonBaseComponent } from 'src/lib/common/common-base.component';
import { CommonMenuService } from 'src/lib/components/common-menu/common-menu.service';
import { CommonBreakpointObserver } from 'src/lib/components/common-breakpointobserver/common-breakpointobserver.service';

import { ApiService } from '../api.service';
import { SlotDto } from './slot.dto';
import { SlotObserversService } from './slot-observers.service';


@Component({
    templateUrl: './slot.component.html',
    providers: [ SlotObserversService ]

})
export class SlotComponent
        extends CommonBaseComponent
        implements OnInit {
    public dto!: SlotDto[];
    public menuShow = false;


    constructor(snackBar: MatSnackBar,
                private readonly route: ActivatedRoute,
                private readonly menuService: CommonMenuService,
                private readonly breakpointObserver: CommonBreakpointObserver,
                private readonly observersService: SlotObserversService,
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
                this.dto = (<SlotDto[]>data['result']).sort(x => x.carId);

                this.observersService
                .onChangedState
                .subscribe((dto: SlotDto) => {
                    console.log('SlotDto', dto);                
                    this.dto[dto.carId - 1] = dto;
                });

                this.observersService.observe();
            }
        });
    }

    
    public menuToggle(): void {
        this.menuService.menuToogle();
    }
}