import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Data } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

import { CommonBaseComponent } from 'src/lib/common/common-base.component';
import { CommonToolbarService } from 'src/lib/components/common-toolbar/common-toolbar.service';
import { ApiErrorResponse } from 'src/lib/common/common-base.service';

import { ApiService } from '../api.service';
import { CommandDto } from './command.dto';
import { CommandObserversService } from './command-observers.service';


@Component({
    templateUrl: './command.component.html',
    providers: [ CommandObserversService ]

})
export class CommandComponent
        extends CommonBaseComponent
        implements OnInit {
    public dto!: CommandDto;
    public dirty: boolean = false;

    
    constructor(snackBar: MatSnackBar,
                private readonly route: ActivatedRoute,
                toolbarService: CommonToolbarService,
                private readonly observersService: CommandObserversService,
                private readonly apiService: ApiService) {
        super(snackBar);
        toolbarService.header = "Command";
    }


    public ngOnInit(): void {
        this.route.data.subscribe({
            next: (data: Data) => {
                this.dto = <CommandDto>data['result'];

                this.observersService
                .onChangedState
                .subscribe((dto: CommandDto) => {
                    console.log('SlotDto', dto);                
                    this.dto = dto;
                });

                this.observersService.observe();
            }
        });
    }


    public dirtyChanged(): void {
        this.dirty = true;
    }


    public write(): void {
        this.apiService.postCommand(this.dto)
        .subscribe({
            next: () => {
                this.snackBarOpen("Command written.");
                this.dirty = false;
             },
            error: (err: ApiErrorResponse) => this.onError(err)
        });
    }
}