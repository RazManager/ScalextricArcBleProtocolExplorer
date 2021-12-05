import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Data } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

import { CommonBaseComponent } from 'src/lib/common/common-base.component';
import { CommonToolbarService } from 'src/lib/components/common-toolbar/common-toolbar.service';

import { TrackDto } from './track.dto';
import { TrackObserversService } from './track-observers.service';


@Component({
    templateUrl: './track.component.html',
    providers: [ TrackObserversService ]

})
export class TrackComponent
        extends CommonBaseComponent
        implements OnInit {
    public dto!: TrackDto;


    constructor(snackBar: MatSnackBar,
                private readonly route: ActivatedRoute,
                toolbarService: CommonToolbarService,
                private readonly observersService: TrackObserversService) {
        super(snackBar);
        toolbarService.header = "Track";
    }


    public ngOnInit(): void {
        this.route.data.subscribe({
            next: (data: Data) => {
                this.dto = <TrackDto>data['result'];

                this.observersService
                .onChangedState
                .subscribe((dto: TrackDto) => {
                    console.log('TrackDto', dto);
                    this.dto = dto;
                });

                this.observersService.observe();
            }
        });
    }
}