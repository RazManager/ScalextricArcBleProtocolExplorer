import { Component, Input } from '@angular/core';

import { CommonMenuService } from '../common-menu/common-menu.service';


@Component({
    selector: 'common-toolbar',
    templateUrl: './common-toolbar.component.html'
})
export class CommonToolbarComponent {
    @Input() header!: string | null;

    constructor(private readonly commonMenuService: CommonMenuService) { }


    public menuToggle(): void {
        this.commonMenuService.menuToogle();
    }
}