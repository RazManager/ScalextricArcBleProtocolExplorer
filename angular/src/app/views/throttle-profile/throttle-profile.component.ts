import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Data } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

import { CommonBaseComponent } from 'src/lib/common/common-base.component';
import { ApiErrorResponse } from 'src/lib/common/common-base.service';

import { ApiService } from '../api.service';
import { ThrottleProfileDto } from './throttle-profile.dto';
import { ThrottleProfileObserversService } from './throttle-profile-observers.service';
import { CommonToolbarService } from 'src/lib/components/common-toolbar/common-toolbar.service';


@Component({
    templateUrl: './throttle-profile.component.html',
    providers: [ ThrottleProfileObserversService ]

})
export class ThrottleProfileComponent
        extends CommonBaseComponent
        implements OnInit {
    public dto!: ThrottleProfileDto[];


    constructor(snackBar: MatSnackBar,
                private readonly route: ActivatedRoute,
                toolbarService: CommonToolbarService,
                private readonly observersService: ThrottleProfileObserversService,
                private readonly apiService: ApiService) {
        super(snackBar);
        toolbarService.header = "Throttle profiles";
    }


    public ngOnInit(): void {
        this.route.data.subscribe({
            next: (data: Data) => {
                this.dto = (<ThrottleProfileDto[]>data['result']).sort((a, b) => a.carId - b.carId);

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


    public write(carId: number): void {
        this.apiService.postThrottleProfile(this.dto[carId - 1])
        .subscribe({
            next: () => {
                this.snackBarOpen("Throttle profile written for ID=" + carId + ".");
             },
            error: (err: ApiErrorResponse) => this.onError(err)
        });
    }
}