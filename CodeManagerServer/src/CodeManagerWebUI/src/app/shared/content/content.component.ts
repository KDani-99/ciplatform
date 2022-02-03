import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'shared-content',
  templateUrl: './content.component.html',
  styleUrls: ['./content.component.scss'],
})
export class ContentComponent implements OnInit {
  @Input() header: string = '';

  constructor() {}

  ngOnInit(): void {}
}
