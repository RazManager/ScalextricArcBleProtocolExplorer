import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Data } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

import { CommonBaseComponent } from '../../../lib/common/common-base.component';
import { CommonToolbarService } from 'src/lib/components/common-toolbar/common-toolbar.service';

import { GattCharacteristicDto, GattCharacteristicFlagDto } from './gatt-characteristic.dto';


@Component({
    templateUrl: './gatt-characteristic.component.html'
})
export class GattCharacteristicComponent
        extends CommonBaseComponent
        implements OnInit {
    public dto!: GattCharacteristicDto[];


    constructor(snackBar: MatSnackBar,
                private readonly route: ActivatedRoute,
                toolbarService: CommonToolbarService) {
        super(snackBar);
        toolbarService.header = "GATT characteristics";
    }
  

    public ngOnInit(): void {
        this.route.data.subscribe({
            next: (data: Data) => {
                this.dto = <GattCharacteristicDto[]>data['result'];
            }
        });
    }


    public flags(flags: GattCharacteristicFlagDto[]): string {
        return flags.map(x => x.flag).join(", ");
    }
}