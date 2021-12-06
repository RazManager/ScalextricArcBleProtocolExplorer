import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Data } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

import { CommonBaseComponent } from 'src/lib/common/common-base.component';

import { PracticeSessionCarIdDto, PracticeSessionLapDto } from './practice-session.dto';
import { PracticeSessionObserversService } from './practice-session-observers.service';
import { CommonToolbarService } from 'src/lib/components/common-toolbar/common-toolbar.service';


@Component({
    templateUrl: './practice-session.component.html',
    providers: [ PracticeSessionObserversService ]

})
export class PracticeSessionComponent
        extends CommonBaseComponent
        implements OnInit {
    public dto!: PracticeSessionCarIdDto[];


    constructor(snackBar: MatSnackBar,
                private readonly route: ActivatedRoute,
                toolbarService: CommonToolbarService,
                private readonly observersService: PracticeSessionObserversService) {
        super(snackBar);
        toolbarService.header = "Practice session";
    }


    public ngOnInit(): void {
        this.route.data.subscribe({
            next: (data: Data) => {
                this.dto = (<PracticeSessionCarIdDto[]>data['result']).sort(x => x.carId);

                this.observersService
                .onChangedState
                .subscribe((dto: PracticeSessionCarIdDto) => {
                    console.log('PracticeSessionCarIdDto', dto);                
                    this.dto[dto.carId - 1] = dto;
                    //dto.latestLaps.sort((a,b) => 0 - (a.lap > b.lap ? 1 : -1));
                });

                this.observersService.observe();
            }
        });
    }


    public latestLaps(carId: number): PracticeSessionLapDto[] {
        return this.dto.find(x => x.carId === carId)!.latestLaps.sort((a,b) => 0 - (a.lap > b.lap ? 1 : -1));
    }
}