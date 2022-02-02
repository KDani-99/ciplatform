import {Component, Input, OnInit, ViewEncapsulation} from '@angular/core';

@Component({
  selector: 'shared-button',
  templateUrl: './button.component.html',
  styleUrls: ['./button.component.scss']
})
export class ButtonComponent implements OnInit {

  @Input() text: string = ''
  @Input() theme: string = 'none';

  constructor() { }

  ngOnInit(): void {
  }

  get className() {
    return `button-container button-container-${this.theme}`
  }

}
