import { Component, Input, OnInit, TemplateRef } from '@angular/core';

@Component({
  selector: 'shared-topbar',
  templateUrl: './topbar.component.html',
  styleUrls: ['./topbar.component.scss'],
})
export class TopbarComponent implements OnInit {
  @Input() buttonsTemplate?: TemplateRef<any> = undefined;
  @Input() imageTemplate?: TemplateRef<any> = undefined;
  @Input() header: string = 'N/A';
  @Input() secondary: boolean = false;

  constructor() {}

  ngOnInit(): void {}
}
