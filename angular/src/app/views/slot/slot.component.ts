import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Data } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

import { CommonBaseComponent } from 'src/lib/common/common-base.component';

import { SlotDto } from './slot.dto';
import { SlotObserversService } from './slot-observers.service';
import { CommonToolbarService } from 'src/lib/components/common-toolbar/common-toolbar.service';


@Component({
    templateUrl: './slot.component.html',
    providers: [ SlotObserversService ]

})
export class SlotComponent
        extends CommonBaseComponent
        implements OnInit {
    public dto!: SlotDto[];


    constructor(snackBar: MatSnackBar,
                private readonly route: ActivatedRoute,
                toolbarService: CommonToolbarService,
                private readonly observersService: SlotObserversService) {
        super(snackBar);
        toolbarService.header = "Slot";
    }


    public ngOnInit(): void {
        this.route.data.subscribe({
            next: (data: Data) => {
                this.dto = (<SlotDto[]>data['result']).sort((a, b) => a.carId - b.carId);

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
}