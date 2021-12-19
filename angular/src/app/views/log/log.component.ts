import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Data } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';

import { CommonBaseComponent } from 'src/lib/common/common-base.component';

import { LogDto } from './log.dto';
import { LogObserversService } from './log-observers.service';
import { CommonToolbarService } from 'src/lib/components/common-toolbar/common-toolbar.service';


@Component({
    templateUrl: './log.component.html',
    providers: [ LogObserversService ]

})
export class LogComponent
        extends CommonBaseComponent
        implements OnInit {
    public items!: LogDto[];
    public dataSource!: MatTableDataSource<LogDto>;
    public columns = ['timestamp', 'logLevel', 'message'];


    constructor(snackBar: MatSnackBar,
                private readonly route: ActivatedRoute,
                toolbarService: CommonToolbarService,
                private readonly observersService: LogObserversService) {
        super(snackBar);
        toolbarService.header = "Log";
    }


    public ngOnInit(): void {
        this.route.data.subscribe({
            next: (data: Data) => {
                this.items = (<LogDto[]>data['result']);
                this.dataSource = new MatTableDataSource<LogDto>(this.items);    

                this.observersService
                .onChangedState
                .subscribe((dto: LogDto) => {
                    this.items.push(dto);
                    this.dataSource = new MatTableDataSource<LogDto>(this.items);    
                });

                this.observersService.observe();
            }
        });
    }
}