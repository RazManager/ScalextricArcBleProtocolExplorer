import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Data } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

import { CommonBaseComponent } from 'src/lib/common/common-base.component';

import { ThrottleProfileDto } from './throttle-profile.dto';
import { ThrottleProfileObserversService } from './throttle-profile-observers.service';
import { CommonToolbarService } from 'src/lib/components/common-toolbar/common-toolbar.service';


@Component({
    templateUrl: './slot.component.html',
    providers: [ ThrottleProfileObserversService ]

})
export class SlotComponent
        extends CommonBaseComponent
        implements OnInit {
    public dto!: ThrottleProfileDto[];


    constructor(snackBar: MatSnackBar,
                private readonly route: ActivatedRoute,
                toolbarService: CommonToolbarService,
                private readonly observersService: ThrottleProfileObserversService) {
        super(snackBar);
        toolbarService.header = "Throttle profiles";
    }


    public ngOnInit(): void {
        this.route.data.subscribe({
            next: (data: Data) => {
                this.dto = (<ThrottleProfileDto[]>data['result']).sort(x => x.carId);

                this.observersService
                .onChangedState
                .subscribe((dto: ThrottleProfileDto) => {
                    console.log('ThrottleProfileDto', dto);                
                    this.dto[dto.carId - 1] = dto;
                });

                this.observersService.observe();
            }
        });
    }
}