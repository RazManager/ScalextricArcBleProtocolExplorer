import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Data } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

import { CommonBaseComponent } from 'src/lib/common/common-base.component';
import { CommonToolbarService } from 'src/lib/components/common-toolbar/common-toolbar.service';
import { ApiErrorResponse } from 'src/lib/common/common-base.service';

import { ApiService } from '../api.service';
import { CarIdDto } from './car-id.dto';
import { CarIdObserversService } from './car-id-observers.service';


@Component({
    templateUrl: './car-id.component.html',
    providers: [ CarIdObserversService ]

})
export class CarIdComponent
        extends CommonBaseComponent
        implements OnInit {
    public dto!: CarIdDto;
                       

    constructor(snackBar: MatSnackBar,
                private readonly route: ActivatedRoute,
                toolbarService: CommonToolbarService,
                private readonly observersService: CarIdObserversService,
                private readonly apiService: ApiService) {
        super(snackBar);
        toolbarService.header = "Car ID";
    }


    public ngOnInit(): void {
        this.route.data.subscribe({
            next: (data: Data) => {
                this.dto = <CarIdDto>data['result'];

                this.observersService
                .onChangedState
                .subscribe((dto: CarIdDto) => {
                    console.log('CarIdDto', dto);                
                    this.dto = dto;
                });

                this.observersService.observe();
            }
        });
    }


    public write(carId: number): void {
        this.dto.carId = carId;
        this.apiService.postCarId(this.dto)
        .subscribe({
            next: () => {
                this.snackBarOpen("Car ID written.");
             },
            error: (err: ApiErrorResponse) => this.onError(err)
        });
    }
}