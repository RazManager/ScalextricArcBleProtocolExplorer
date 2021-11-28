import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

import { CommonBaseComponent } from 'src/lib/common/common-base.component';
import { CommonToolbarService } from 'src/lib/components/common-toolbar/common-toolbar.service';
import { ApiErrorResponse } from 'src/lib/common/common-base.service';

import { ApiService } from '../api.service';
import { CommandService } from './command.service';
import { CommandObserversService } from './command-observers.service';


@Component({
    templateUrl: './command.component.html',
    providers: [ CommandObserversService ]

})
export class CommandComponent
        extends CommonBaseComponent {


    constructor(snackBar: MatSnackBar,
                private readonly route: ActivatedRoute,
                toolbarService: CommonToolbarService,
                public readonly commandService: CommandService,
                private readonly apiService: ApiService) {
        super(snackBar);
        toolbarService.header = "Command";
    }

    public write(): void {
        this.apiService.postCommand(this.commandService.dto)
        .subscribe({
            next: () => {
                this.snackBarOpen("Command written.");
             },
            error: (err: ApiErrorResponse) => this.onError(err)
        });
    }
}