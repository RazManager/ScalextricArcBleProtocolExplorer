import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Data } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

import { CommonBaseComponent } from '../../../lib/common/common-base.component';
import { CommonMenuService } from '../../../lib/components/common-menu/common-menu.service';

import { SystemDto } from './system.dto';
import { ApiService } from '../api.service';


@Component({
    templateUrl: './system.component.html'
})
export class SystemComponent
        extends CommonBaseComponent
        implements OnInit {
    public dto!: SystemDto;


    constructor(snackBar: MatSnackBar,
                private readonly route: ActivatedRoute,
                private readonly menuService: CommonMenuService,
                private readonly apiService: ApiService) {
        super(snackBar);
    }


    public ngOnInit(): void {
        this.route.data.subscribe({
            next: (data: Data) => {
                this.dto = <SystemDto>data['result'];
            }
        });
    }


    public menu(): void {
        this.menuService.menuToogle();
    }
}