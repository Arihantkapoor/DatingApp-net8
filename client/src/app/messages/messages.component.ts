import { Component, inject, OnInit } from '@angular/core';
import { MessageService } from '../_services/message.service';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { FormsModule } from '@angular/forms';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { TimeagoModule } from 'ngx-timeago';
import { RouterLink } from '@angular/router';
import { Message } from '../_models/message';

@Component({
  selector: 'app-messages',
  standalone: true,
  imports: [PaginationModule, FormsModule, ButtonsModule, TimeagoModule, RouterLink],
  templateUrl: './messages.component.html',
  styleUrl: './messages.component.css'
})
export class MessagesComponent implements OnInit {
  messageService = inject(MessageService);
  pageNumber = 1;
  pageSize = 5;
  container = 'Inbox';
  isOutbox = this.container === 'Outbox';

  ngOnInit(): void {
      this.loadMessages();
  }

  getRoute(message: Message){
    if(this.container === 'Outbox') return `/members/${message.recipientUsername}`;
    else return `/members/${message.senderUsername}`;
  }

  loadMessages(){
    this.messageService.getMessages(this.pageNumber, this.pageSize, this.container);
  }

  deleteMessage(id: number){
    this.messageService.deleteMessage(id).subscribe({
      next: _ => {
        this.messageService.paginatedResult.update( prev =>{
          if(prev && prev.items){
           prev.items.splice(prev.items.findIndex(m=> m.id === id),1);
           return prev;
          }
            return prev;
        })
      }
    })
  }

  pageChanged(event: any){
    if(this.pageNumber!== event.page){
      this.pageNumber = event.page;
      this.loadMessages();
    }
  }

}
