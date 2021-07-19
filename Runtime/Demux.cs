using System;

namespace ev {
    public class Token {
        private bool subscribed;

        public Token() {
            this.subscribed = true;
        }

        public bool IsSubscribed() {
            return subscribed;
        }

        public void Cancel() {
            this.subscribed = false;
        }
    }

    public class Demux<T> {
        private class EventNode {
            public EventNode next;
            public Token token;
            public Action<T> receiver;
        }

        private EventNode head;
        private EventNode tail;

        public void Register(Token token, Action<T> receiver) {
            if (token == null || receiver == null) {
                return;
            }

            var node = new EventNode();
            node.token = token;
            node.receiver = receiver;

            if (head == null) {
                head = node;
            } else {
                tail.next = node;
            }

            tail = node;
        }

        public void Push(T ev) {
            if (tail == null) {
                return;
            }

            EventNode prev = null;
            var node = head;
            while (true) {
                if (!node.token.IsSubscribed()) {
                    if (prev == null) {
                        head = node.next;
                        if (head == null) {
                            tail = null;
                            break;
                        }
                    } else {
                        prev.next = node.next;
                        if (node.next == null) {
                            tail = prev;
                            break;
                        }
                    }

                    node = node.next;
                    continue;
                } else {
                    node.receiver(ev);
                }

                if (node.next == null) {
                    break;
                }

                prev = node;
                node = node.next;
            }
        }
    }
}
