import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'shared-data',
  templateUrl: './data.component.html',
  styleUrls: ['./data.component.scss'],
})
export class DataComponent implements OnInit {
  @Input() header: string = '';
  @Input() value: string = '';

  constructor() {}

  ngOnInit(): void {}
}
