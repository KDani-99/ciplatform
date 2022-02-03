import { Component, Input, OnInit, TemplateRef } from '@angular/core';

@Component({
  selector: 'shared-popup',
  templateUrl: './popup.component.html',
  styleUrls: ['./popup.component.scss'],
})
export class PopupComponent implements OnInit {
  @Input() contentTemplate?: TemplateRef<any>;

  constructor() {}

  ngOnInit(): void {}
}
