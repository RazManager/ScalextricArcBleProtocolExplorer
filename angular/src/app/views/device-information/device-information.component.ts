import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Data } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

import { CommonBaseComponent } from '../../../lib/common/common-base.component';
import { CommonToolbarService } from 'src/lib/components/common-toolbar/common-toolbar.service';

import { DeviceInformationDto } from './device-information.dto';


@Component({
    templateUrl: './device-information.component.html'
})
export class DeviceInformationComponent
        extends CommonBaseComponent
        implements OnInit {
    public dto!: DeviceInformationDto;


    constructor(snackBar: MatSnackBar,
                private readonly route: ActivatedRoute,
                toolbarService: CommonToolbarService) {
        super(snackBar);
        toolbarService.header = "Device information";
    }


    public ngOnInit(): void {
        this.route.data.subscribe({
            next: (data: Data) => {
                this.dto = <DeviceInformationDto>data['result'];
            }
        });
    }
}