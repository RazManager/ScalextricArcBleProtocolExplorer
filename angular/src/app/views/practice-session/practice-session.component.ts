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
                const testUnsorted = (<PracticeSessionCarIdDto[]>data['result']);
                console.log("testUnsorted", testUnsorted.map(x => x.carId).join(", "));
                const testSorted = (<PracticeSessionCarIdDto[]>data['result']);
                console.log("testSorted", testSorted.map(x => x.carId).join(", "));
                this.dto = (<PracticeSessionCarIdDto[]>data['result']).sort((a, b) => a.carId - b.carId);
                console.log("dto", this.dto.map(x => x.carId).join(", "));

                this.observersService
                .onChangedState
                .subscribe((dto: PracticeSessionCarIdDto) => {
                    console.log('PracticeSessionCarIdDto', dto);                
                    this.dto[dto.carId - 1] = dto;
                });

                this.observersService.observe();
            }
        });
    }
}