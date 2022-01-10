import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Data } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

import { CommonBaseComponent } from '../../../lib/common/common-base.component';
import { CommonToolbarService } from 'src/lib/components/common-toolbar/common-toolbar.service';

import { SystemInformationDto } from './system-information.dto';


@Component({
    templateUrl: './system-information.component.html'
})
export class SystemInformationComponent
        extends CommonBaseComponent
        implements OnInit {
    public dto!: SystemInformationDto;


    constructor(snackBar: MatSnackBar,
                private readonly route: ActivatedRoute,
                toolbarService: CommonToolbarService) {
        super(snackBar);
        toolbarService.header = "System information";
    }


    public ngOnInit(): void {
        this.route.data.subscribe({
            next: (data: Data) => {
                this.dto = <SystemInformationDto>data['result'];
            }
        });
    }
}