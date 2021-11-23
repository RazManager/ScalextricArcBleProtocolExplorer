import { Component, Input, Output, EventEmitter, OnInit, ContentChild, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { BreakpointState } from '@angular/cdk/layout';
import { CommonBreakpointObserver } from '../common-breakpointobserver/common-breakpointobserver.service';
import { CommonMenuService } from '../common-menu/common-menu.service';


@Component({
    selector: 'common-container-list',
    templateUrl: './common-container-list.component.html'
})
export class CommonContainerListComponent implements OnInit {
    @Input() header!: string;
    @Input() routerLinkAdd: string | null = null;
    @Output() addClick: EventEmitter<void> = new EventEmitter<void>();
    public menuShow = false;
    public addShow = false;


    constructor(private readonly router: Router,
                private readonly commonMenuService: CommonMenuService,        
                private readonly commonBreakpointObserver: CommonBreakpointObserver) { }


    public ngOnInit(): void {
        this.menuShow = this.commonBreakpointObserver.isNarrow;
        this.addShow = this.routerLinkAdd !== null || this.addClick.observers.length > 0;

        this.commonBreakpointObserver
            .observeNarrow
            .subscribe({
                next:  (state: BreakpointState) => this.menuShow = state.matches
            });
    }


    public menuToogle(): void {
        this.commonMenuService.menuToogle();
    }

    
    public add(): void {
        if (this.routerLinkAdd) {
            this.router.navigate([this.routerLinkAdd, 'add']);
        }
        if (this.addClick.observers.length > 0) {
            this.addClick.emit();
        }
    }
}
