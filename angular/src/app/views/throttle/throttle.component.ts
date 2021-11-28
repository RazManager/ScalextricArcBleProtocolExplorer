import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Data } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

import { CommonBaseComponent } from 'src/lib/common/common-base.component';
import { CommonToolbarService } from 'src/lib/components/common-toolbar/common-toolbar.service';

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


    constructor(snackBar: MatSnackBar,
                private readonly route: ActivatedRoute,
                toolbarService: CommonToolbarService,
                private readonly observersService: ThrottleObserversService) {
        super(snackBar);
        toolbarService.header = "Throttle";
    }


    public ngOnInit(): void {
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
}