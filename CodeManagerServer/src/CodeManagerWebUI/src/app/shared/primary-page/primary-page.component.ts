import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'shared-primary-page',
  templateUrl: './primary-page.component.html',
  styleUrls: ['./primary-page.component.scss'],
})
export class PrimaryPageComponent implements OnInit {
  @Input() header: string = '';

  constructor() {}

  ngOnInit(): void {}
}
