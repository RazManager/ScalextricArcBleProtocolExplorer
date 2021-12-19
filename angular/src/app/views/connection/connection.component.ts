import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Data } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

import { CommonBaseComponent } from 'src/lib/common/common-base.component';
import { CommonToolbarService } from 'src/lib/components/common-toolbar/common-toolbar.service';
import { ApiErrorResponse } from 'src/lib/common/common-base.service';

import { ApiService } from '../api.service';
import { ConnectionDto } from './connection.dto';
import { ConnectionObserversService } from './connection-observers.service';


@Component({
    templateUrl: './connection.component.html',
    providers: [ ConnectionObserversService ]

})
export class ConnectionComponent
        extends CommonBaseComponent
        implements OnInit {
    public dto!: ConnectionDto;


    constructor(snackBar: MatSnackBar,
                private readonly route: ActivatedRoute,
                toolbarService: CommonToolbarService,
                private readonly observersService: ConnectionObserversService,
                private readonly apiService: ApiService) {
        super(snackBar);
        toolbarService.header = "Connection";
    }


    public ngOnInit(): void {
        this.route.data.subscribe({
            next: (data: Data) => {
                this.dto = <ConnectionDto>data['result'];

                this.observersService
                .onChangedState
                .subscribe((dto: ConnectionDto) => {
                    this.dto = dto;
                });

                this.observersService.observe();
            }
        });
    }


    public change(): void {
        this.apiService.putConnection(this.dto)
        .subscribe({
            next: () => {
                if (this.dto.connect) {
                    this.snackBarOpen("Connecting...");
                }
                else {
                    this.snackBarOpen("Disconnecting...");
                }

             },
            error: (err: ApiErrorResponse) => this.onError(err)
        });
    }
}