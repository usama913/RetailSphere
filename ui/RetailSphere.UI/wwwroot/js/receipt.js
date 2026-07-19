// Small JS-interop helpers for the POS receipt (Print / Download as PDF / Share).
//
// "Print" and "Download PDF" both call retailSphereReceipt.print(html): it opens a
// brand-new, blank browser window/tab (not the current page) and writes a small
// self-contained receipt document into it, then calls THAT window's own print().
// This is what makes the browser's real print dialog actually appear as a genuine
// popup with "Save as PDF" available as a destination — printing the current page
// in place (the previous approach) does not reliably trigger that dialog inside a
// Blazor WASM app, since it depends on the print CSS/DOM state of the main app
// rather than a clean, dedicated document.
//
// "Share" uses the Web Share API where available (mobile/most modern desktop
// browsers) and falls back to copying a plain-text summary to the clipboard otherwise.

window.retailSphereReceipt = {
    print: function (receiptHtml) {
        var printWindow = window.open('', '_blank', 'width=420,height=720');
        if (!printWindow) {
            // Popup blocked by the browser — nothing else we can safely do here.
            return;
        }

        var document_ = printWindow.document;
        document_.open();
        document_.write(
            '<!DOCTYPE html><html><head><meta charset="utf-8" /><title>Receipt</title><style>' +
            'body{font-family:"Courier New",Courier,monospace;font-size:13px;color:#111;margin:20px;}' +
            '.rs-receipt{max-width:320px;margin:0 auto;}' +
            '.rs-center{text-align:center;}' +
            '.rs-row{display:flex;justify-content:space-between;gap:12px;}' +
            'hr{border:none;border-top:1px dashed #999;margin:8px 0;}' +
            '</style></head><body>' + receiptHtml + '</body></html>'
        );
        document_.close();

        var triggerPrint = function () {
            printWindow.focus();
            printWindow.print();
        };

        // onload is the reliable signal that the written document has finished
        // parsing, but not every browser fires it consistently after
        // document.write — the short timeout is a harmless backup trigger.
        printWindow.onload = triggerPrint;
        setTimeout(triggerPrint, 300);
    },

    share: async function (title, text) {
        if (navigator.share) {
            try {
                await navigator.share({ title: title, text: text });
                return 'shared';
            } catch (e) {
                // AbortError when the user cancels the share sheet — not a failure.
                return 'cancelled';
            }
        }

        if (navigator.clipboard && navigator.clipboard.writeText) {
            await navigator.clipboard.writeText(text);
            return 'copied';
        }

        return 'unsupported';
    }
};
