import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Data } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

import { CommonBaseComponent } from '../../../lib/common/common-base.component';
import { CommonMenuService } from '../../../lib/components/common-menu/common-menu.service';

import { DeviceDto } from './device.dto';
import { ApiService } from '../api.service';


@Component({
    templateUrl: './device.component.html'
})
export class DeviceComponent
        extends CommonBaseComponent
        implements OnInit {
    public dto!: DeviceDto;


    constructor(snackBar: MatSnackBar,
                private readonly route: ActivatedRoute,
                private readonly menuService: CommonMenuService,
                private readonly apiService: ApiService) {
        super(snackBar);
    }


    public ngOnInit(): void {
        this.route.data.subscribe({
            next: (data: Data) => {
                this.dto = <DeviceDto>data['result'];
            }
        });
    }


    public menu(): void {
        this.menuService.menuToogle();
    }
}