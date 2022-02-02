import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'shared-input',
  templateUrl: './input.component.html',
  styleUrls: ['./input.component.scss'],
})
export class InputComponent implements OnInit {
  public value: string = '';

  @Input() type: 'text' | 'password' | 'email' = 'text';
  @Input() header: string = '';
  @Input() placeholder: string = '';

  constructor() {}

  ngOnInit(): void {}
}
