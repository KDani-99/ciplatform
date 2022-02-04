import { Component, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'shared-checkbox',
  templateUrl: './checkbox.component.html',
  styleUrls: ['./checkbox.component.scss'],
})
export class CheckboxComponent implements OnInit {
  @Input() value?: string;
  @Input() id?: string;

  @Output() isChecked: boolean = false;

  constructor() {}

  ngOnInit(): void {}
}
